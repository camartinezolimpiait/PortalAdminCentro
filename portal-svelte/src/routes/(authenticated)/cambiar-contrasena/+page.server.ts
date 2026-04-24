import type { Actions } from './$types';
import { sisecAuthClient } from '$lib/server/api-client';
import { fail } from '@sveltejs/kit';

export const actions: Actions = {
default: async ({ request, locals }) => {
const formData = await request.formData();
const currentPassword = formData.get('currentPassword')?.toString() || '';
const newPassword = formData.get('newPassword')?.toString() || '';
const confirmPassword = formData.get('confirmPassword')?.toString() || '';

if (!currentPassword || !newPassword || !confirmPassword) {
return fail(400, { error: 'Todos los campos son obligatorios' });
}
if (newPassword !== confirmPassword) {
return fail(400, { error: 'Las contraseñas no coinciden' });
}
if (newPassword.length < 8) {
return fail(400, { error: 'La contraseña debe tener al menos 8 caracteres' });
}

try {
await sisecAuthClient.post('Login/ChangePassword', {
userId: locals.user?.userId,
currentPassword,
newPassword
}, locals.token || '');
return { success: true };
} catch {
return fail(500, { error: 'Error al cambiar la contraseña' });
}
}
};
