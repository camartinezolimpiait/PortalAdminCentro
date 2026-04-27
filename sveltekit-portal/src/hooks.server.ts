import type { Handle } from '@sveltejs/kit';
import { redirect } from '@sveltejs/kit';
import { getClientSession } from '$lib/server/session';
import { isPublicRoute, isAuthorized } from '$lib/server/rbac';

export const handle: Handle = async ({ event, resolve }) => {
	// Step 1: Hydrate event.locals.user from HttpOnly session cookie
	const user = getClientSession(event.cookies);
	event.locals.user = user;

	const { pathname } = event.url;

	// Step 2: Allow public routes without authentication
	if (isPublicRoute(pathname)) {
		return resolve(event);
	}

	// Step 3: Require authentication for non-public routes
	if (!user) {
		throw redirect(303, '/login');
	}

	// Step 4: RBAC - check user roles against route rules
	if (!isAuthorized(pathname, user.roles)) {
		throw redirect(303, '/403-unauthorized');
	}

	return resolve(event);
};
