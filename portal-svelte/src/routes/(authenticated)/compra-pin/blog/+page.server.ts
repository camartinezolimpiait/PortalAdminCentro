import type { PageServerLoad } from './$types';
import { strapiClient } from '$lib/server/api-client';

export const load: PageServerLoad = async () => {
try {
const articles = await strapiClient.get('articles?populate=*');
return { articles: Array.isArray(articles) ? articles : [] };
} catch {
return { articles: [] };
}
};
