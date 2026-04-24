import type { PageServerLoad } from './$types';
import { portalAdminClient, getApiToken } from '$lib/server/api-client';

export const load: PageServerLoad = async ({ locals }) => {
try {
const apiToken = await getApiToken();
const today = new Date().toISOString().split('T')[0];
const citas = await portalAdminClient.post(
'Agenda/ConsultarCitas',
{ centroId: locals.user?.centroId || 0, fecha: today },
apiToken.tokenBearer
);
return { citas: Array.isArray(citas) ? citas : [] };
} catch {
return { citas: [], error: 'Error al cargar la agenda' };
}
};
