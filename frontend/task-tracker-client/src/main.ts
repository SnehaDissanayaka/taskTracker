import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { AppComponent } from './app/app.component';
import { clientIdInterceptor } from './app/client-id.interceptor';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

bootstrapApplication(AppComponent, {
  providers: [provideHttpClient(withInterceptors([clientIdInterceptor])), provideAnimationsAsync()],
}).catch((err) => console.error(err));
