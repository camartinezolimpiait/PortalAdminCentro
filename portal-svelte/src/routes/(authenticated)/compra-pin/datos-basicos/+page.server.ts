import type { PageServerLoad, Actions } from './$types';
import { portalAdminClient, getApiToken } from '$lib/server/api-client';
import { fail, redirect } from '@sveltejs/kit';

export const load: PageServerLoad = async () => {
let tiposDocumento: { id: number; nombre: string }[] = [];
try {
const apiToken = await getApiToken();
tiposDocumento = await portalAdminClient.get('Catalogo/TiposDocumento', apiToken.tokenBearer);
} catch {
tiposDocumento = [
{ id: 1, nombre: 'Cédula de Ciudadanía' },
{ id: 2, nombre: 'Cédula de Extranjería' },
{ id: 3, nombre: 'Pasaporte' },
{ id: 4, nombre: 'Tarjeta de Identidad' }
];
}
return { tiposDocumento };
};

export const actions: Actions = {
default: async ({ request }) => {
const formData = await request.formData();
const tipoDocumento = formData.get('tipoDocumento')?.toString() || '';
const numeroDocumento = formData.get('numeroDocumento')?.toString() || '';
const primerNombre = formData.get('primerNombre')?.toString() || '';
const primerApellido = formData.get('primerApellido')?.toString() || '';

if (!tipoDocumento || !numeroDocumento || !primerNombre || !primerApellido) {
return fail(400, { error: 'Los campos marcados con * son obligatorios', tipoDocumento, numeroDocumento, primerNombre, primerApellido });
}

throw redirect(303, '/compra-pin/datos-personales');
}
};
