import type { Actions, PageServerLoad } from './$types';
import { fail, redirect } from '@sveltejs/kit';
import { authenticate, buildSession } from '$lib/server/auth';
import { createSession } from '$lib/server/session';
import { DEFAULT_LANDING_PAGE } from '$lib/server/constants';

export const load: PageServerLoad = async ({ locals }) => {
  if (locals.user) throw redirect(303, DEFAULT_LANDING_PAGE);
  return {};
};

export const actions: Actions = {
  default: async ({ request, cookies }) => {
    const formData = await request.formData();
    const userName = formData.get('userName')?.toString() ?? '';
    const password = formData.get('password')?.toString() ?? '';
    const plataforma = formData.get('plataforma')?.toString() ?? 'CRC';

    if (!userName || !password) {
      return fail(400, { error: 'Usuario y contraseña son requeridos', userName });
    }

    const response = await authenticate({ userName, password, plataforma });

    if (!response.success) {
      return fail(401, { error: response.message || 'Credenciales inválidas', userName });
    }

    if (response.mustChangePassword) {
      return fail(403, { mustChangePassword: true, tempPassword: response.tempPassword, userName });
    }

    const session = buildSession(response, plataforma);
    if (!session) {
      return fail(500, { error: 'Error al crear sesión', userName });
    }

    createSession(cookies, session);
    throw redirect(303, DEFAULT_LANDING_PAGE);
  }
};
