import type { PageServerLoad } from './$types';
import { portalAdminClient, getApiToken } from '$lib/server/api-client';

export const load: PageServerLoad = async ({ locals }) => {
try {
const apiToken = await getApiToken();
const categorias = await portalAdminClient.get(
`Categoria/GetCategoriasByCentro?idCentro=${locals.user?.centroId || 0}`,
apiToken.tokenBearer
);
return { categorias: Array.isArray(categorias) ? categorias : [] };
} catch {
return { categorias: [] };
}
};
