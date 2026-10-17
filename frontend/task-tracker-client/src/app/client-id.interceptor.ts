import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../environments/environment';
import { ClientIdService } from './client-id.service';

// Only calls to our own API get the header, so the ID is never sent to third parties.
export const clientIdInterceptor: HttpInterceptorFn = (req, next) => {
  if (!req.url.startsWith(environment.apiBaseUrl)) return next(req);
  return next(req.clone({ setHeaders: { 'X-Client-Id': inject(ClientIdService).id } }));
};
