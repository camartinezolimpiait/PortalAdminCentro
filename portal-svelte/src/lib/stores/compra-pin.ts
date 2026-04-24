// ===================================================================
// CompraPin State Store
// Replaces: ApplicationShared + CompraPin state management from Blazor
// Manages multi-step purchase flow state
// ===================================================================

import { writable } from 'svelte/store';
import type { DatosBasicos, DatosFacturacion, PasosCompraPin } from '$lib/types/compra-pin';

export interface CompraPinState {
	pasoActual: number;
	pasos: PasosCompraPin[];
	datosBasicos: DatosBasicos | null;
	datosFacturacion: DatosFacturacion | null;
	categoriaId: number | null;
	tramiteId: number | null;
	medioPago: string | null;
	valorTotal: number;
	referenciaPago: string | null;
}

const initialState: CompraPinState = {
	pasoActual: 1,
	pasos: [
		{ paso: 1, nombre: 'Datos Básicos', ruta: '/compra-pin/datos-basicos', completado: false },
		{
			paso: 2,
			nombre: 'Datos Personales',
			ruta: '/compra-pin/datos-personales',
			completado: false
		},
		{ paso: 3, nombre: 'Categorías', ruta: '/compra-pin/categorias', completado: false },
		{ paso: 4, nombre: 'Trámite', ruta: '/compra-pin/tramite', completado: false },
		{ paso: 5, nombre: 'Resumen', ruta: '/compra-pin/resumen', completado: false },
		{ paso: 6, nombre: 'Medios de Pago', ruta: '/compra-pin/medios-pago', completado: false },
		{ paso: 7, nombre: 'Confirmación', ruta: '/compra-pin/confirmacion', completado: false }
	],
	datosBasicos: null,
	datosFacturacion: null,
	categoriaId: null,
	tramiteId: null,
	medioPago: null,
	valorTotal: 0,
	referenciaPago: null
};

function createCompraPinStore() {
	const { subscribe, set, update } = writable<CompraPinState>({ ...initialState });

	return {
		subscribe,
		setDatosBasicos: (datos: DatosBasicos) =>
			update((s) => ({
				...s,
				datosBasicos: datos,
				pasos: s.pasos.map((p) => (p.paso === 1 ? { ...p, completado: true } : p)),
				pasoActual: 2
			})),
		setDatosFacturacion: (datos: DatosFacturacion) =>
			update((s) => ({
				...s,
				datosFacturacion: datos,
				pasos: s.pasos.map((p) => (p.paso === 2 ? { ...p, completado: true } : p)),
				pasoActual: 3
			})),
		setCategoria: (categoriaId: number) =>
			update((s) => ({
				...s,
				categoriaId,
				pasos: s.pasos.map((p) => (p.paso === 3 ? { ...p, completado: true } : p)),
				pasoActual: 4
			})),
		setTramite: (tramiteId: number) =>
			update((s) => ({
				...s,
				tramiteId,
				pasos: s.pasos.map((p) => (p.paso === 4 ? { ...p, completado: true } : p)),
				pasoActual: 5
			})),
		setMedioPago: (medioPago: string) =>
			update((s) => ({
				...s,
				medioPago,
				pasos: s.pasos.map((p) => (p.paso === 6 ? { ...p, completado: true } : p)),
				pasoActual: 7
			})),
		setValorTotal: (valor: number) => update((s) => ({ ...s, valorTotal: valor })),
		setReferenciaPago: (ref: string) => update((s) => ({ ...s, referenciaPago: ref })),
		goToStep: (paso: number) => update((s) => ({ ...s, pasoActual: paso })),
		reset: () => set({ ...initialState })
	};
}

export const compraPinStore = createCompraPinStore();
