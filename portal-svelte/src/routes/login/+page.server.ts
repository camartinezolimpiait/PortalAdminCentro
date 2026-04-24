import { fail, redirect } from '@sveltejs/kit';
import type { Actions, PageServerLoad } from './$types';
import { authenticateUser, createSessionCookie, getClientes, recoverPassword } from '$lib/server/auth';
import { getApiToken } from '$lib/server/api-client';

export const load: PageServerLoad = async ({ locals, url }) => {
	// Already authenticated? Redirect to dashboard
	if (locals.user?.authenticated) {
		throw redirect(303, '/pines');
	}

	const clientes = await getClientes();
	const recoverMode = url.searchParams.get('recover') === 'true';

	return {
		clientes,
		recoverMode
	};
};

export const actions: Actions = {
	login: async ({ request, cookies }) => {
		const formData = await request.formData();
		const userName = formData.get('userName')?.toString() || '';
		const passWord = formData.get('passWord')?.toString() || '';
		const plataforma = formData.get('plataforma')?.toString() || '';

		if (!userName || !passWord || !plataforma) {
			return fail(400, {
				error: 'Todos los campos son obligatorios',
				userName,
				plataforma
			});
		}

		const result = await authenticateUser(userName, passWord, plataforma);

		if (!result.success || !result.user) {
			return fail(401, {
				error: result.message,
				userName,
				plataforma
			});
		}

		// Get API token for the session
		let token = '';
		try {
			const apiToken = await getApiToken();
			token = apiToken?.tokenBearer || '';
		} catch {
			// Non-critical: session works without API token
		}

		// Create secure session cookie
		createSessionCookie(cookies, result.user, token);

		// Check if password change is required
		// (would need additional logic from the auth response)

		throw redirect(303, '/pines');
	},

	recoverPassword: async ({ request }) => {
		const formData = await request.formData();
		const userName = formData.get('userName')?.toString() || '';
		const plataforma = formData.get('plataforma')?.toString() || '';

		if (!userName || !plataforma) {
			return fail(400, {
				error: 'Nombre de usuario y tipo de negocio son obligatorios',
				userName,
				plataforma,
				recover: true
			});
		}

		const result = await recoverPassword(userName, plataforma);

		if (!result.success) {
			return fail(400, {
				error: result.message,
				userName,
				plataforma,
				recover: true
			});
		}

		return {
			success: true,
			email: result.email,
			message: result.message,
			recover: true
		};
	}
};
