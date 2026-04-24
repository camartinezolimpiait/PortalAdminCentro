import type { Actions } from './$types';
import { sisecAuthClient } from '$lib/server/api-client';
import { fail } from '@sveltejs/kit';

export const actions: Actions = {
default: async ({ request, locals }) => {
const formData = await request.formData();
const userName = formData.get('userName')?.toString() || '';
const email = formData.get('email')?.toString() || '';
const firstName = formData.get('firstName')?.toString() || '';
const lastName = formData.get('lastName')?.toString() || '';
const perfil = formData.get('perfil')?.toString() || '';

if (!userName || !email || !firstName || !lastName) {
return fail(400, { error: 'Todos los campos son obligatorios' });
}

try {
await sisecAuthClient.post('User/Create', {
userName, email, firstName, lastName, perfil,
centroId: locals.user?.centroId
}, locals.token || '');
return { success: true };
} catch {
return fail(500, { error: 'Error al crear el usuario' });
}
}
};
