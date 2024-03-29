import { Component, Inject, OnInit, ViewChild } from '@angular/core';
//import {HttpClient, HttpParams} from '@angular/common/http';
import { MatTableDataSource} from '@angular/material/table';
import { MatPaginator, PageEvent} from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

import { City } from './city';
import { CityService } from './city.service';
import { ApiResult } from '../base.service';

@Component({
    selector: 'city',
    templateUrl: 'city.component.html',
    styleUrls:['./city.component.css']
})

export class CityComponent implements OnInit {
    public displayColumns:string[]=['id', 'name', 'lat', 'lon', 'countryName'];
    public cities: MatTableDataSource<City>;

    defaultPageIndex: number=0;
    defaultPageSize: number=10;
    public defaultSortColumn: string="name";
    public defaultSortOrder: string="asc";
    defaultFilterColumn: string="name";
    filterQuery: string=null;

    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;

    filterTextChanged: Subject<string> = new Subject<string>();

    constructor(private cityService: CityService){ }
    //@Inject('BASE_URL') private baseUrl:string,private http:HttpClient, 

    ngOnInit() {
        this.loadData();
     }

     loadData(query:string = null){
        var pageEvent=new PageEvent();
        pageEvent.pageIndex= this.defaultPageIndex;
        pageEvent.pageSize= this.defaultPageSize;

        this.filterQuery=query!=null? query : this.filterQuery;
        this.getData(pageEvent);
     }

     getData(event: PageEvent){
         var sortColumn: string= this.sort? 
                        this.sort.active : this.defaultSortColumn;
         var sortOrder: string= this.sort?
                         this.sort.direction: this.defaultSortOrder;
         var filterColumn= this.filterQuery? this.defaultFilterColumn : null;
         var filterQuery= this.filterQuery ? this.filterQuery : null;

        this.cityService.getData<ApiResult<City>>(event.pageIndex, event.pageSize,
             sortColumn, sortOrder, filterColumn, filterQuery).subscribe(result=>{
                this.paginator.length=result.totalCount;
                this.paginator.pageIndex=result.pageIndex;
                this.paginator.pageSize=result.pageSize;
                this.cities=new MatTableDataSource<City>(result.data);
            },error=>console.error(error));
    }

        //debounce filter text changes
    onFilterTextChanged(filterText: string){
        if(this.filterTextChanged.observers.length === 0){
            this.filterTextChanged.pipe(debounceTime(1000),
                distinctUntilChanged())
            .subscribe(query => {
                console.log('search called!!');
                this.loadData(query);
            });
        }
        this.filterTextChanged.next(filterText);
    }
}


