import type { PageServerLoad, Actions } from './$types';
import { superTransporteClient } from '$lib/server/api-client';
import { fail } from '@sveltejs/kit';

export const load: PageServerLoad = async ({ locals }) => {
try {
const pqrsf = await superTransporteClient.get(`pqrsf/${locals.user?.centroId || 0}`);
return { pqrsf: Array.isArray(pqrsf) ? pqrsf : [] };
} catch {
return { pqrsf: [] };
}
};

export const actions: Actions = {
create: async ({ request, locals }) => {
const formData = await request.formData();
const tipo = formData.get('tipo')?.toString() || '';
const asunto = formData.get('asunto')?.toString() || '';
const descripcion = formData.get('descripcion')?.toString() || '';

if (!tipo || !asunto || !descripcion) {
return fail(400, { error: 'Todos los campos son obligatorios' });
}

try {
await superTransporteClient.post('pqrsf', {
centroId: locals.user?.centroId || 0,
tipo, asunto, descripcion
});
return { success: true };
} catch {
return fail(500, { error: 'Error al crear PQRSF' });
}
}
};
