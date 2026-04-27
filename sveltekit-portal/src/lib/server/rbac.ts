import type { UserRole } from '$lib/types/auth';

interface RouteRule {
  pattern: string;
  roles: UserRole[];
}

const ACCESS_RULES: RouteRule[] = [
  // SuperTransporte – Director & Auditor
  { pattern: '/supertransporte/*', roles: ['Director', 'Auditor'] },
  // Reportes – Director & Auditor
  { pattern: '/reportes', roles: ['Director', 'Auditor'] },
  // Configuracion – Director only
  { pattern: '/configuracion/*', roles: ['Director'] },
  // Agenda – Instructor & Director
  { pattern: '/agenda', roles: ['Instructor', 'Director'] },
  // Pines – any authenticated user
  { pattern: '/pines/*', roles: [] },
  { pattern: '/pines', roles: [] },
  // Consultar Facturacion
  { pattern: '/consultar-facturacion/*', roles: ['Director', 'Auditor'] },
  { pattern: '/consultar-facturacion', roles: ['Director', 'Auditor'] },
];

const PUBLIC_ROUTES = [
  '/', '/login', '/change-password',
  '/compradepin', '/comprapincda',
  '/403-unauthorized'
];

const PUBLIC_PREFIXES = [
  '/compradepin/', '/comprapincda/',
  '/api/auth'
];

export function isPublicRoute(pathname: string): boolean {
  if (PUBLIC_ROUTES.includes(pathname)) return true;
  return PUBLIC_PREFIXES.some((prefix) => pathname.startsWith(prefix));
}

export function isAuthorized(pathname: string, userRoles: UserRole[]): boolean {
  if (isPublicRoute(pathname)) return true;

  const rule = ACCESS_RULES.find((r) => matchRoute(r.pattern, pathname));
  if (!rule) return true; // No explicit rule = any authenticated user
  if (rule.roles.length === 0) return true; // Empty = any authenticated
  return rule.roles.some((role) => userRoles.includes(role));
}

export function getRouteRoles(pathname: string): UserRole[] | null {
  const rule = ACCESS_RULES.find((r) => matchRoute(r.pattern, pathname));
  if (!rule) return null;
  return rule.roles.length > 0 ? rule.roles : null;
}

function matchRoute(pattern: string, pathname: string): boolean {
  if (pattern.endsWith('/*')) {
    const prefix = pattern.slice(0, -1);
    return pathname.startsWith(prefix) || pathname === prefix.slice(0, -1);
  }
  return pattern === pathname;
}
