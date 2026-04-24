// See https://svelte.dev/docs/kit/types#app.d.ts
import type { SessionUser } from '$lib/types/models';

declare global {
	namespace App {
		interface Error {
			message: string;
			code?: string;
		}

		interface Locals {
			user: SessionUser | null;
			token: string | null;
		}

		interface PageData {
			user: SessionUser | null;
		}

		// interface PageState {}
		// interface Platform {}
	}
}

export {};
