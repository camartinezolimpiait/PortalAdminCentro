import type { PageServerLoad } from './$types';
import { portalAdminClient, getApiToken } from '$lib/server/api-client';
import type { ResponseInfoPinEstadoActivos } from '$lib/types/pines';

export const load: PageServerLoad = async ({ locals, url }) => {
const page = parseInt(url.searchParams.get('page') || '1');
const pageSize = parseInt(url.searchParams.get('pageSize') || '10');

try {
const apiToken = await getApiToken();
const data = await portalAdminClient.post<ResponseInfoPinEstadoActivos>(
'Pin/ConsultarPinesPorEstado',
{
idCentro: locals.user?.centroId || 0,
estado: 1, // Active
fechaInicio: '',
fechaFin: '',
pagina: page,
registrosPagina: pageSize
},
apiToken.tokenBearer
);
return { pines: data, currentPage: page, pageSize };
} catch (error) {
console.error('Error loading active pines:', error);
return { pines: null, currentPage: page, pageSize, error: 'Error al cargar los pines activos' };
}
};
