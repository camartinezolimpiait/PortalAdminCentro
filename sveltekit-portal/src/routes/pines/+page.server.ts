import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ url }) => {
  const estado = url.searchParams.get('estado') ?? 'activos';
  return {
    estado,
    pines: [] // API integration pending
  };
};
