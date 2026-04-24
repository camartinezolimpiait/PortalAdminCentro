// ===================================================================
// hooks.server.ts - Central RBAC Guard
// Replaces: [Authorize], [Authorize(Roles="...")] attributes from Blazor
// All route protection is enforced server-side before page resolution
// ===================================================================

import { redirect, type Handle } from '@sveltejs/kit';
import { getSession } from '$lib/server/auth';

// ===================================================================
// RBAC Route Map
// Maps routes to required roles. Routes not listed are PROTECTED
// by default (require authentication but no specific role).
// ===================================================================

/** Public routes that don't require authentication */
const PUBLIC_ROUTES = new Set([
	'/login',
	'/api/auth/login',
	'/api/auth/logout',
	'/api/auth/recover-password',
	'/403-unauthorized'
]);

/** Route -> allowed roles mapping */
const ROLE_ROUTES: Record<string, string[]> = {
	// Pines module - all authenticated users
	'/pines': ['CRC', 'CEA', 'CDA', 'Director', 'Auditor', 'Investigador'],
	'/pines/activos': ['CRC', 'CEA', 'CDA', 'Director', 'Auditor', 'Investigador'],
	'/pines/usados': ['CRC', 'CEA', 'CDA', 'Director', 'Auditor', 'Investigador'],
	'/pines/devoluciones': ['CRC', 'CEA', 'CDA', 'Director', 'Auditor'],
	'/pines/dispersiones': ['CRC', 'CEA', 'CDA', 'Director', 'Auditor'],
	'/pines/certificado-ingreso': ['CRC', 'CEA', 'CDA', 'Director'],
	'/pines/busqueda': ['CRC', 'CEA', 'CDA', 'Director', 'Auditor', 'Investigador'],
	'/pines/asociados': ['CRC', 'CEA', 'CDA', 'Director', 'Auditor', 'Investigador'],

	// CompraPin module
	'/compra-pin': ['CRC', 'CEA', 'Director', 'Auditor'],
	'/compra-pin-cda': ['CDA', 'Director'],

	// Agendamiento module
	'/agendamiento': ['CRC', 'CEA', 'CDA', 'Director', 'Instructor'],
	'/agendamiento/agenda': ['CRC', 'CEA', 'CDA', 'Director', 'Instructor'],
	'/agendamiento/configuracion': ['CRC', 'CEA', 'CDA', 'Director'],

	// Facturacion module
	'/facturacion': ['CRC', 'CEA', 'CDA', 'Director', 'Auditor'],
	'/facturacion/configurar': ['CRC', 'CEA', 'CDA', 'Director'],
	'/facturacion/consultar': ['CRC', 'CEA', 'CDA', 'Director', 'Auditor'],

	// SuperTransporte module
	'/supertransporte': ['CRC', 'CEA', 'Director', 'Auditor', 'Investigador'],
	'/supertransporte/centro': ['CRC', 'Director', 'Auditor', 'Investigador'],
	'/supertransporte/centro-cea': ['CEA', 'Director', 'Auditor', 'Investigador'],
	'/supertransporte/crc': ['CRC', 'Director', 'Auditor', 'Investigador'],
	'/supertransporte/cea': ['CEA', 'Director', 'Auditor', 'Investigador'],
	'/supertransporte/pqrsf': ['CRC', 'CEA', 'Director', 'Auditor'],

	// PowerBI reports
	'/powerbi': ['CRC', 'CEA', 'CDA', 'Director', 'Auditor'],

	// Administration
	'/crear-usuario': ['Director'],
	'/cambiar-contrasena': ['CRC', 'CEA', 'CDA', 'Director', 'Auditor', 'Investigador', 'Instructor'],
	'/gestionar-pin': ['CRC', 'CEA', 'CDA', 'Director'],

	// Validation pages
	'/valida-pin-crc': ['CRC', 'Director'],
	'/valida-pin-cea': ['CEA', 'Director'],

	// Payment
	'/pago-cuota': ['CRC', 'CEA', 'CDA', 'Director'],
	'/devolucion': ['CRC', 'CEA', 'CDA', 'Director', 'Auditor']
};

/**
 * Check if a route is public (no auth required)
 */
function isPublicRoute(pathname: string): boolean {
	if (PUBLIC_ROUTES.has(pathname)) return true;

	// Static assets and API routes for auth are public
	if (pathname.startsWith('/_app/') || pathname.startsWith('/favicon')) return true;

	return false;
}

/**
 * Check if user has required roles for a route.
 * Uses prefix matching to handle nested routes.
 */
function hasRequiredRole(pathname: string, userRoles: string[]): boolean {
	// Find the most specific matching route
	let matchedRoute: string | null = null;
	let matchLength = 0;

	for (const route of Object.keys(ROLE_ROUTES)) {
		if (pathname === route || pathname.startsWith(route + '/')) {
			if (route.length > matchLength) {
				matchedRoute = route;
				matchLength = route.length;
			}
		}
	}

	// No specific role requirement found → allow any authenticated user
	if (!matchedRoute) return true;

	const allowedRoles = ROLE_ROUTES[matchedRoute];
	return userRoles.some((role) => allowedRoles.includes(role));
}

export const handle: Handle = async ({ event, resolve }) => {
	const pathname = event.url.pathname;

	// Skip auth for public routes
	if (isPublicRoute(pathname)) {
		event.locals.user = null;
		event.locals.token = null;
		return resolve(event);
	}

	// Get session from cookie
	const session = getSession(event.cookies);

	// No session → redirect to login
	if (!session || !session.user?.authenticated) {
		if (pathname.startsWith('/api/')) {
			return new Response(JSON.stringify({ error: 'Unauthorized' }), {
				status: 401,
				headers: { 'Content-Type': 'application/json' }
			});
		}
		throw redirect(303, '/login');
	}

	// Set user and token in locals for downstream use
	event.locals.user = session.user;
	event.locals.token = session.token;

	// Check role-based access
	if (!hasRequiredRole(pathname, session.user.roles)) {
		if (pathname.startsWith('/api/')) {
			return new Response(JSON.stringify({ error: 'Forbidden' }), {
				status: 403,
				headers: { 'Content-Type': 'application/json' }
			});
		}
		throw redirect(303, '/403-unauthorized');
	}

	return resolve(event);
};
