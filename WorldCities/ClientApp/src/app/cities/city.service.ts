import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BaseService, ApiResult } from '../base.service';
import { Observable } from 'rxjs';

import { City } from './city';

@Injectable({
  providedIn: 'root',
})
export class CityService extends BaseService {
  constructor(
    http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    super(http, baseUrl);
  }

  getData<ApiResult>(
    pageIndex: number, pageSize: number, sortColumn: string,
    sortOrder: string, filterColumn: string, filterQuery: string
  ): Observable<ApiResult> {
    var url = this.baseUrl + 'api/Cities/' + pageIndex+ '/' +
                pageSize +'/'+ sortColumn +'/'+ sortOrder;

    if(filterQuery)
        url=url + '/' + filterColumn + '/' + filterQuery;
    return this.http.get<ApiResult>(url);
  }

  get<City>(id): Observable<City> {
    var url = this.baseUrl + "api/Cities/" + id;
    return this.http.get<City>(url);
  }

  put<City>(item): Observable<City> {
    var url = this.baseUrl + "api/Cities/" + item.id;
    return this.http.put<City>(url, item);
  }

  post<City>(item): Observable<City> {
    var url = this.baseUrl + "api/Cities";
    return this.http.post<City>(url, item);
  }

  getCountries<ApiResult>(pageIndex: number, pageSize: number,
    sortColumn: string, sortOrder: string, filterColumn: string,
    filterQuery: string): Observable<ApiResult> {

    var url = this.baseUrl + 'api/Countries/' + pageIndex+ '/' +
    pageSize +'/'+ sortColumn +'/'+ sortOrder;

    if(filterQuery)
        url=url + '/' + filterColumn + '/' + filterQuery;

    return this.http.get<ApiResult>(url);
  }

  isDupeCity(item): Observable<boolean> {
    var url = this.baseUrl + "api/Cities/IsDupeCity";
    return this.http.post<boolean>(url, item);
  }
}