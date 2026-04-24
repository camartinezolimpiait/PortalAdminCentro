import { redirect } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ locals }) => {
if (locals.user?.authenticated) {
// Redirect authenticated users to their first menu item or pines
const firstMenu = locals.user.menuPrincipal?.[0];
if (firstMenu?.ruta) {
throw redirect(303, firstMenu.ruta);
}
throw redirect(303, '/pines');
}
throw redirect(303, '/login');
};
