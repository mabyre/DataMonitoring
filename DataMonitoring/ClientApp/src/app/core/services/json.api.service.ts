import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { delay, map, catchError, retry } from 'rxjs/operators';
import { Observable, throwError, } from 'rxjs';
import { OidcSecurityService } from 'angular-auth-oidc-client';

import { Configuration } from '@app/core/configuration';

@Injectable({
  providedIn: 'root',
})
export class JsonApiService {

  private headers: HttpHeaders = new HttpHeaders();

  constructor(private http: HttpClient,
    private configuration: Configuration,
    private securityService: OidcSecurityService) { }

  public get(actionUrl): Observable<any> {

    this.setHeaders();
    let url = this.getBaseUrl() + actionUrl;

    if (localStorage.getItem('culture') !== null) {
      url = url + '?ui_locales=' + localStorage.getItem('culture');
    }

    return this.http.get(url, { headers: this.headers })
      .pipe(
        delay(this.configuration.apiTimeout),
        retry(this.configuration.apiRetry),
        map((data: any) => (data.data || data)),
        catchError(this.handleError)
      );
  }

  public getAll<T>(actionUrl): Observable<T[]> {

    this.setHeaders();
    let url = this.getBaseUrl() + actionUrl;

    if (localStorage.getItem('culture') !== null) {
      url = url + '?ui_locales=' + localStorage.getItem('culture');
    }

    return this.http.get<T[]>(url, { headers: this.headers })
      .pipe(
        delay(this.configuration.apiTimeout),
        retry(this.configuration.apiRetry),
        catchError(this.handleError)
      );
  }

  public getById<T>(actionUrl, id: any): Observable<T> {

    this.setHeaders();
    let url = this.getBaseUrl() + actionUrl + `/${id}`;

    if (localStorage.getItem('culture') !== null) {
      url = url + '?ui_locales=' + localStorage.getItem('culture');
    }

    return this.http.get<T>(url, { headers: this.headers })
      .pipe(
        delay(this.configuration.apiTimeout),
        retry(this.configuration.apiRetry),
        catchError(this.handleError)
      );
  }

  public addJSON(actionUrl, itemToAdd: any): Observable<any> {

    this.setHeaders();
    return this.http.post<any>(this.getBaseUrl() + actionUrl, JSON.stringify(itemToAdd), { headers: this.headers })
      .pipe(
        delay(this.configuration.apiTimeout),
        retry(this.configuration.apiRetry),
        catchError(this.handleError)
      );
  }

  public add(actionUrl, itemToAdd: any): Observable<any> {

    this.setHeaders();
    return this.http.post<any>(this.getBaseUrl() + actionUrl, itemToAdd, { headers: this.headers })
      .pipe(
        delay(this.configuration.apiTimeout),
        retry(this.configuration.apiRetry),
        catchError(this.handleError)
      );
  }

  public update(actionUrl, id: any, itemToUpdate: any): Observable<any> {

    this.setHeaders();
    return this.http.put<any>(this.getBaseUrl() + actionUrl + `/${id}`, itemToUpdate, { headers: this.headers })
      .pipe(
        delay(this.configuration.apiTimeout),
        retry(this.configuration.apiRetry),
        catchError(this.handleError)
      );
  }

  public delete(actionUrl, id: any): Observable<any> {

    this.setHeaders();
    return this.http.delete<any>(this.getBaseUrl() + actionUrl + `/${id}`, { headers: this.headers })
      .pipe(
        delay(this.configuration.apiTimeout),
        retry(this.configuration.apiRetry),
        catchError(this.handleError)
      );
  }

  public duplicate(actionUrl, id: any): Observable<any> {

    this.setHeaders();
    return this.http.post<any>(this.getBaseUrl() + actionUrl + `/${id}`, { headers: this.headers })
      .pipe(
        delay(this.configuration.apiTimeout),
        retry(this.configuration.apiRetry),
        catchError(this.handleError)
      );
  }

  private getBaseUrl() {

    if (this.configuration.apiServerUrl != null) {
      return this.configuration.apiServerUrl;
    }
    return location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '') + '/';
  }

  private setHeaders() {

    this.headers = new HttpHeaders();
    this.headers.set('Content-Type', this.configuration.headerContentType);
    this.headers.set('Accept', this.configuration.headerAccept);
   
    const token = this.securityService.getToken();
    if (token !== '') {
      const tokenValue = 'Bearer ' + token;
      this.headers = this.headers.append('Authorization', tokenValue);
    }
  }

  private handleError(error: any) {
    let errMsg = (error.message) 
      ? error.message 
      : error.status 
          ? `${error.status} - ${error.statusText}` 
          : 'Server error';
      
    if (error.error) {
      errMsg += " : " + error.error;
    }

    console.error(errMsg); // log to console instead
    return throwError(errMsg);
  }
}
