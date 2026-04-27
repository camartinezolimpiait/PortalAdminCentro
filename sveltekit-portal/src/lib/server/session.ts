import { dev } from '$app/environment';
import type { Cookies } from '@sveltejs/kit';
import type { ServerSession, UserSession } from '$lib/types/auth';
import { SESSION_COOKIE_NAME, SESSION_MAX_AGE_SECONDS } from './constants';

/**
 * SECURITY NOTE: In production, replace base64url encoding with proper
 * encryption using iron-session or jose (JWE). Base64 is NOT encryption.
 */
function encodeSession(session: ServerSession): string {
  return Buffer.from(JSON.stringify(session)).toString('base64url');
}

function decodeSession(encoded: string): ServerSession | null {
  try {
    const json = Buffer.from(encoded, 'base64url').toString('utf-8');
    const parsed = JSON.parse(json) as ServerSession;
    if (parsed.expiresAt && Date.now() > parsed.expiresAt) {
      return null;
    }
    return parsed;
  } catch {
    return null;
  }
}

export function createSession(cookies: Cookies, session: ServerSession): void {
  const encoded = encodeSession(session);
  cookies.set(SESSION_COOKIE_NAME, encoded, {
    path: '/',
    httpOnly: true,
    secure: !dev,
    sameSite: 'lax',
    maxAge: SESSION_MAX_AGE_SECONDS
  });
}

export function getServerSession(cookies: Cookies): ServerSession | null {
  const encoded = cookies.get(SESSION_COOKIE_NAME);
  if (!encoded) return null;
  return decodeSession(encoded);
}

export function getClientSession(cookies: Cookies): UserSession | null {
  const session = getServerSession(cookies);
  if (!session) return null;
  const { tokenBearer, expiresAt, ...clientData } = session;
  return clientData as UserSession;
}

export function destroySession(cookies: Cookies): void {
  cookies.delete(SESSION_COOKIE_NAME, { path: '/' });
}

export function getToken(cookies: Cookies): string | null {
  const session = getServerSession(cookies);
  return session?.tokenBearer ?? null;
}
