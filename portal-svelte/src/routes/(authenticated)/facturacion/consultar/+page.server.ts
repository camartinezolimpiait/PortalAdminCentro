import type { PageServerLoad, Actions } from './$types';
import { portalAdminClient, getApiToken } from '$lib/server/api-client';
import { fail } from '@sveltejs/kit';

export const load: PageServerLoad = async () => {
return { facturas: [] };
};

export const actions: Actions = {
search: async ({ request, locals }) => {
const formData = await request.formData();
const fechaInicio = formData.get('fechaInicio')?.toString() || '';
const fechaFin = formData.get('fechaFin')?.toString() || '';
const estado = formData.get('estado')?.toString() || '';

try {
const apiToken = await getApiToken();
const facturas = await portalAdminClient.post(
'Facturacion/Consultar',
{ centroId: locals.user?.centroId || 0, fechaInicio, fechaFin, estado, pagina: 1, registrosPagina: 20 },
apiToken.tokenBearer
);
return { facturas: Array.isArray(facturas) ? facturas : [] };
} catch {
return fail(500, { error: 'Error al consultar facturas' });
}
}
};
