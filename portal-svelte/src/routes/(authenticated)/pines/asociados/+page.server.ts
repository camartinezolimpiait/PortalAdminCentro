import type { PageServerLoad, Actions } from './$types';
import { portalAdminClient, getApiToken } from '$lib/server/api-client';
import { fail } from '@sveltejs/kit';

export const load: PageServerLoad = async () => {
return { results: null };
};

export const actions: Actions = {
search: async ({ request, locals }) => {
const formData = await request.formData();
const tipoDocumento = formData.get('tipoDocumento')?.toString() || '';
const numeroDocumento = formData.get('numeroDocumento')?.toString() || '';

if (!tipoDocumento || !numeroDocumento) {
return fail(400, { error: 'Todos los campos son obligatorios' });
}

try {
const apiToken = await getApiToken();
const results = await portalAdminClient.post(
'Pin/ConsultarPinesAsociados',
{ idCentro: locals.user?.centroId || 0, tipoDocumento, numeroDocumento },
apiToken.tokenBearer
);
return { results, tipoDocumento, numeroDocumento };
} catch {
return fail(500, { error: 'Error al consultar pines asociados' });
}
}
};
