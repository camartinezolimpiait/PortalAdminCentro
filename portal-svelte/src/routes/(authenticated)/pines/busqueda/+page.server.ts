import type { PageServerLoad, Actions } from './$types';
import { portalAdminClient, getApiToken } from '$lib/server/api-client';
import { fail } from '@sveltejs/kit';

export const load: PageServerLoad = async () => {
return { results: null };
};

export const actions: Actions = {
search: async ({ request, locals }) => {
const formData = await request.formData();
const searchType = formData.get('searchType')?.toString() || 'pin';
const searchValue = formData.get('searchValue')?.toString() || '';

if (!searchValue) {
return fail(400, { error: 'Ingrese un valor de búsqueda' });
}

try {
const apiToken = await getApiToken();
let results;
if (searchType === 'pin') {
results = await portalAdminClient.post(
'Pin/ConsultarDetallePin',
{ idPin: parseInt(searchValue) || 0 },
apiToken.tokenBearer
);
} else {
results = await portalAdminClient.post(
'Pin/ConsultarPinesPorDocumento',
{ tipoDocumento: '', numeroDocumento: searchValue, idCentro: locals.user?.centroId || 0 },
apiToken.tokenBearer
);
}
return { results, searchType, searchValue };
} catch {
return fail(500, { error: 'Error al buscar pines' });
}
}
};
