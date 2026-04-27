import type { UserSession } from '$lib/types/auth';

declare global {
	namespace App {
		interface Error {
			message: string;
			code?: string;
		}
		interface Locals {
			user: UserSession | null;
		}
		interface PageData {
			user: UserSession | null;
		}
	}
}

export {};
