// ===================================================================
// Auth Service - Server-side only
// Replaces: Login.razor @code, ApplicationSevice session logic
// Handles authentication, session cookie management
// ===================================================================

import { sisecAuthClient, portalAdminClient, getApiToken } from '$lib/server/api-client';
import { getConfig } from '$lib/server/config';
import type { SessionUser, MenuItem, ClienteDTO, Perfil } from '$lib/types/models';
import type { Cookies } from '@sveltejs/kit';

const SESSION_COOKIE = 'sisec_session';
const SESSION_MAX_AGE = 60 * 60 * 8; // 8 hours

interface LoginResult {
	success: boolean;
	message: string;
	user?: SessionUser;
}

interface AuthSession {
	user: SessionUser;
	token: string;
	expiresAt: number;
}

/**
 * Authenticate user against SisecAuth API.
 * Replaces: Login.razor OnSubmitLogin logic
 */
export async function authenticateUser(
	userName: string,
	passWord: string,
	plataforma: string
): Promise<LoginResult> {
	try {
		// Get API gateway token first
		const apiToken = await getApiToken();
		if (!apiToken?.tokenBearer) {
			return { success: false, message: 'Error obteniendo token de acceso' };
		}

		// Authenticate user against SisecAuth
		const authResponse = await sisecAuthClient.post<{
			tokenBearer: string;
			message: string;
			userId: string;
			userName: string;
			mustChangePassword: boolean;
			firstName: string;
		}>(
			'Login/Authenticate',
			{ userName, passWord, plataforma },
			apiToken.tokenBearer
		);

		if (!authResponse?.tokenBearer) {
			return {
				success: false,
				message: authResponse?.message || 'Credenciales inválidas'
			};
		}

		// Fetch user menu and permissions
		let menuItems: MenuItem[] = [];
		let menuPrincipal: MenuItem[] = [];
		let subMenuItems: MenuItem[] = [];

		try {
			const config = getConfig();
			const menuResponse = await portalAdminClient.get<MenuItem[]>(
				`Menu/GetMenuByUser?userId=${authResponse.userId}&aplicacionGuid=${config.guidAplicacion}`,
				authResponse.tokenBearer
			);

			if (Array.isArray(menuResponse)) {
				menuItems = menuResponse.filter((m) => m.padre === 0);
				menuPrincipal = menuResponse.filter(
					(m) => m.padre === 0 && m.esOpcionMenu === 'True'
				);
				subMenuItems = menuResponse.filter((m) => m.padre !== 0);
			}
		} catch {
			// Menu loading is non-critical, continue with empty menus
			console.warn('Could not load user menu items');
		}

		// Determine user roles based on platform and profile
		const roles = determineRoles(plataforma, menuItems);

		const sessionUser: SessionUser = {
			userId: authResponse.userId,
			userName: authResponse.userName || userName,
			firstName: authResponse.firstName || userName,
			plataforma,
			centroId: 0, // Will be populated from centro lookup
			codigoRunt: null,
			clienteId: 0,
			perfilId: null,
			roles,
			menuItems,
			menuPrincipal,
			subMenuItems,
			authenticated: true
		};

		return { success: true, message: 'OK', user: sessionUser };
	} catch (error) {
		console.error('Authentication error:', error);
		return {
			success: false,
			message: 'Error de conexión con el servidor de autenticación'
		};
	}
}

/**
 * Determine user roles from platform and menu permissions.
 * Maps to the RBAC roles: Auditor, Investigador, Director, Instructor
 */
function determineRoles(plataforma: string, menuItems: MenuItem[]): string[] {
	const roles: string[] = [];

	// Base role from platform
	roles.push(plataforma); // CRC, CEA, CDA, Armas

	// Infer roles from menu items available to user
	const menuNames = menuItems.map((m) => m.pagina?.toLowerCase() || '');

	if (menuNames.some((n) => n.includes('auditor') || n.includes('auditoria'))) {
		roles.push('Auditor');
	}
	if (menuNames.some((n) => n.includes('investigador') || n.includes('investigacion'))) {
		roles.push('Investigador');
	}
	if (menuNames.some((n) => n.includes('director') || n.includes('administrador'))) {
		roles.push('Director');
	}
	if (menuNames.some((n) => n.includes('instructor'))) {
		roles.push('Instructor');
	}

	// Default role if none detected
	if (roles.length === 1) {
		roles.push('Director'); // Default to Director if no specific role detected
	}

	return roles;
}

/**
 * Create session cookie with encrypted session data.
 */
export function createSessionCookie(
	cookies: Cookies,
	user: SessionUser,
	token: string
): void {
	const session: AuthSession = {
		user,
		token,
		expiresAt: Date.now() + SESSION_MAX_AGE * 1000
	};

	cookies.set(SESSION_COOKIE, Buffer.from(JSON.stringify(session)).toString('base64'), {
		path: '/',
		httpOnly: true,
		secure: true,
		sameSite: 'lax',
		maxAge: SESSION_MAX_AGE
	});
}

/**
 * Get session from cookie.
 */
export function getSession(cookies: Cookies): AuthSession | null {
	const cookieValue = cookies.get(SESSION_COOKIE);
	if (!cookieValue) return null;

	try {
		const session = JSON.parse(
			Buffer.from(cookieValue, 'base64').toString('utf-8')
		) as AuthSession;

		if (session.expiresAt < Date.now()) {
			clearSession(cookies);
			return null;
		}

		return session;
	} catch {
		clearSession(cookies);
		return null;
	}
}

/**
 * Clear session cookie.
 */
export function clearSession(cookies: Cookies): void {
	cookies.delete(SESSION_COOKIE, { path: '/' });
}

/**
 * Get list of business types (clients) for login form.
 * Replaces: Login.razor clienteDtoGral population
 */
export async function getClientes(): Promise<ClienteDTO[]> {
	try {
		const apiToken = await getApiToken();
		if (!apiToken?.tokenBearer) return getDefaultClientes();

		const clientes = await portalAdminClient.get<ClienteDTO[]>(
			'Cliente/GetClientes',
			apiToken.tokenBearer
		);

		return Array.isArray(clientes) ? clientes : getDefaultClientes();
	} catch {
		return getDefaultClientes();
	}
}

/**
 * Default client types when API is unavailable.
 */
function getDefaultClientes(): ClienteDTO[] {
	return [
		{ id: 1, nombre: 'CRC', applicationId: '' },
		{ id: 2, nombre: 'CEA', applicationId: '' },
		{ id: 3, nombre: 'CDA', applicationId: '' }
	];
}

/**
 * Recover password flow.
 * Replaces: Login.razor OnSubmitRecoverPass
 */
export async function recoverPassword(
	userName: string,
	plataforma: string
): Promise<{ success: boolean; email: string; message: string }> {
	try {
		const apiToken = await getApiToken();
		if (!apiToken?.tokenBearer) {
			return { success: false, email: '', message: 'Error de conexión' };
		}

		const response = await sisecAuthClient.post<{
			email: string;
			emailConfirmed: boolean;
			message: string;
		}>(
			'Login/RecoverPassword',
			{ userName, plataforma },
			apiToken.tokenBearer
		);

		return {
			success: true,
			email: response.email || '',
			message: response.message || 'Se ha enviado una contraseña temporal al correo registrado'
		};
	} catch {
		return {
			success: false,
			email: '',
			message: 'Error al recuperar la contraseña'
		};
	}
}
