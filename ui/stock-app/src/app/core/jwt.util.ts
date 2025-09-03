export function getToken(): string | null {
  // prefer sessionStorage when "remember me" is off
  return sessionStorage.getItem('jwt') ?? localStorage.getItem('jwt');
}

export function parseJwt<T = any>(token: string): T | null {
  try {
    const payload = token.split('.')[1];
    const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
    return JSON.parse(decodeURIComponent(escape(json)));
  } catch { return null; }
}

export function isExpired(token: string): boolean {
  const payload = parseJwt<any>(token);
  if (!payload?.exp) return true;
  const now = Math.floor(Date.now() / 1000);
  return payload.exp <= now;
}

export function getRolesFromToken(token: string): string[] {
  const p = parseJwt<any>(token) ?? {};
  // .NET puts roles under ClaimTypes.Role URI; sometimes also "role"/"roles"
  const uri = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
  const roles = p[uri] ?? p.role ?? p.roles ?? [];
  return Array.isArray(roles) ? roles : (roles ? [roles] : []);
}
