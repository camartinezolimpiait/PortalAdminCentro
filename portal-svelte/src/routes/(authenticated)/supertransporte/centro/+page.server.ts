import type { PageServerLoad } from './$types';
import { superTransporteClient } from '$lib/server/api-client';

export const load: PageServerLoad = async ({ locals }) => {
try {
const centroInfo = await superTransporteClient.get(`centro/${locals.user?.centroId || 0}`);
return { centroInfo };
} catch {
return { centroInfo: null, error: 'Error al cargar información del centro' };
}
};
