import type { PageServerLoad } from './$types';
import { portalAdminClient, getApiToken } from '$lib/server/api-client';

export const load: PageServerLoad = async ({ locals, url }) => {
const page = parseInt(url.searchParams.get('page') || '1');
const pageSize = parseInt(url.searchParams.get('pageSize') || '10');
try {
const apiToken = await getApiToken();
const data = await portalAdminClient.post(
'Pin/ConsultarDispersiones',
{ idCentro: locals.user?.centroId || 0, fechaInicio: '', fechaFin: '', pagina: page, registrosPagina: pageSize },
apiToken.tokenBearer
);
return { dispersiones: data, currentPage: page, pageSize };
} catch {
return { dispersiones: null, currentPage: page, pageSize, error: 'Error al cargar dispersiones' };
}
};
