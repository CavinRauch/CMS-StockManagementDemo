import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

import { registerLocaleData } from '@angular/common';
import localeEnZa from '@angular/common/locales/en-ZA';

// Make ZA number/date/currency formats available
registerLocaleData(localeEnZa);

bootstrapApplication(AppComponent, appConfig).catch((err) => console.error(err));
