// ===================================================================
// Environment Configuration
// Replaces: Aplication/AppSettings.cs + appsettings.json
// All sensitive config is server-side only via $env/static/private
// ===================================================================

import { env } from '$env/dynamic/private';

export interface AppConfig {
	uriSisecAuth: string;
	uriSisecParametization: string;
	nombreAplicativo: string;
	endPointApiStrapi: string;
	endPointApiSuperT: string;
	endPointApiVigilados: string;
	tokenStrapi: string;
	tokenSuperVigilados: string;
	apiPortalAdministrativo: {
		url: string;
		userName: string;
		userPassword: string;
	};
	apiFrontMiLicencia: {
		url: string;
		userName: string;
		userPassword: string;
		encrypt: { key: string; iv: string };
	};
	secretKey: string;
	captchaVerifyUrl: string;
	isRecaptchaActive: boolean;
	guidAplicacion: string;
}

/**
 * Returns app configuration from environment variables.
 * In production, these should be set via env vars.
 * Defaults match the original appsettings.json for development.
 */
export function getConfig(): AppConfig {
	return {
		uriSisecAuth: env.URI_SISEC_AUTH ?? 'https://olnlbpreapssi01:9056/SisecAut/',
		uriSisecParametization:
			env.URI_SISEC_PARAMETIZATION ?? 'https://olnlbpreapssi01:9056/SisecAdmin/',
		nombreAplicativo: env.NOMBRE_APLICATIVO ?? 'SISEC Recepción',
		endPointApiStrapi: env.ENDPOINT_API_STRAPI ?? 'http://20.185.227.206:1337/api/',
		endPointApiSuperT: env.ENDPOINT_API_SUPERT ?? 'http://179.1.200.166:8081/sicov/',
		endPointApiVigilados:
			env.ENDPOINT_API_VIGILADOS ?? 'http://179.1.200.166:4445/InformacionSICOV/',
		tokenStrapi: env.TOKEN_STRAPI ?? '',
		tokenSuperVigilados: env.TOKEN_SUPER_VIGILADOS ?? '',
		apiPortalAdministrativo: {
			url: env.API_PORTAL_ADMIN_URL ?? 'https://olnlbpreapssi01:9056/ApiGatewaySisec/',
			userName: env.API_PORTAL_ADMIN_USER ?? '',
			userPassword: env.API_PORTAL_ADMIN_PASS ?? ''
		},
		apiFrontMiLicencia: {
			url: env.API_FRONT_MILICENCIA_URL ?? 'https://olsrvprewtssi01:7112/BackEnd/',
			userName: env.API_FRONT_MILICENCIA_USER ?? '',
			userPassword: env.API_FRONT_MILICENCIA_PASS ?? '',
			encrypt: {
				key: env.MILICENCIA_ENCRYPT_KEY ?? '',
				iv: env.MILICENCIA_ENCRYPT_IV ?? ''
			}
		},
		secretKey: env.RECAPTCHA_SECRET_KEY ?? '',
		captchaVerifyUrl: 'https://www.google.com/recaptcha/api/siteverify',
		isRecaptchaActive: env.IS_RECAPTCHA_ACTIVE === 'true',
		guidAplicacion: env.GUID_APLICACION ?? '3288CC9D-C578-4908-9D68-C077A9EC5428'
	};
}
