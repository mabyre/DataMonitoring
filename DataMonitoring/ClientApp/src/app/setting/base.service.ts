import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { JsonApiService } from '../core/services';


export class BaseService<T> {

  protected actionUrl: string;

  constructor(protected jsonApiService: JsonApiService, private apiControllerName: string) {
    this.actionUrl = 'api/' + apiControllerName;
  }

  // GET
  get(): Observable<T[]> {
    return this.jsonApiService.getAll<T>(this.actionUrl);
  }

  // GET
  getById(id: number): Observable<T> {
    return this.jsonApiService.getById<T>(this.actionUrl, id);
  }

  // POST
  post(item: any): Observable<any> {
    return this.jsonApiService.add(this.actionUrl, item);
  }

  // PUT
  put(id: any, item: any): Observable<any> {
    return this.jsonApiService.update(this.actionUrl, id, item);
  }

  // DELETE
  delete(id: number): Observable<any> {
    return this.jsonApiService.delete(this.actionUrl, id);
  }

  getColorList(): Observable<any[]> {
    return this.jsonApiService.getAll('api/color');
  }

  // protected handleError(error: HttpErrorResponse) {
  //   if (error.error instanceof ErrorEvent) {
  //     // A client-side or network error occurred. Handle it accordingly.
  //     console.error('An error occurred:', error.error.message);
  //   } else {
  //     // The backend returned an unsuccessful response code.
  //     // The response body may contain clues as to what went wrong,
  //     console.error(
  //       `Backend returned code ${error.status}, ` +
  //       `body was: ${error.error}`);
  //   }
  //   // return an observable with a user-facing error message
  //   return throwError(
  //     `Status: ${error.status} ${error.statusText}. ${error.error}.`);
  // }
}
