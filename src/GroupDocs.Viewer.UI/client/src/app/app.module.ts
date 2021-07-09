import { Injectable, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { APP_BASE_HREF } from '@angular/common';

import { AppComponent } from './app.component';

import { ConfigService, FileCredentials } from '@groupdocs.examples.angular/common-components';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ViewerConfigService, ViewerService, ViewerModule } from '@groupdocs.examples.angular/viewer';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

declare global {
  interface Window {
    apiEndpoint:string;
    uiSettingsPath:string;
  }
}

@Injectable({
  providedIn: 'root'
})
export class NoThumbnailsViewerService extends ViewerService {

  constructor(http: HttpClient, config: ConfigService) {
    super(http, config);
  }

  loadThumbnails(credentials: FileCredentials) : Observable<Object> {
    return new BehaviorSubject({});
  }
}

export class StaticViewerConfigService {
    public updatedConfig: Observable<any> = new BehaviorSubject({
        pageSelector: true,
        download: true,
        upload: true,
        print: true,
        browse: true,
        rewrite: true,
        enableRightClick: true,
        filesDirectory: "",
        fontsDirectory: "",
        defaultDocument: "1-pages.doc",
        watermarkText: "",
        preloadPageCount: 3,
        zoom: true,
        search: true,
        thumbnails: true,
        rotate: false,
        htmlMode: false,
        cache: true,
        saveRotateState: false,
        printAllowed: true,
        showGridLines: true,
    }).asObservable();

    load(): Promise<void> {
        return Promise.resolve();
    }
}

export function endPoint() {
  let config = new ConfigService();
  config.apiEndpoint = window.apiEndpoint;
  config.getViewerApiEndpoint = () => window.apiEndpoint;
  config.getConfigEndpoint = () => window.uiSettingsPath;
  return config;
}

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    ViewerModule,
    FontAwesomeModule
  ],
  providers: [
    { provide: APP_BASE_HREF, useValue: '/' },
    { provide: ConfigService, useFactory: endPoint },
    /*{ provide: ViewerConfigService, useClass: StaticViewerConfigService },*/
    { provide: ViewerService, useClass: NoThumbnailsViewerService },
    { provide: 'WINDOW', useValue: window },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
