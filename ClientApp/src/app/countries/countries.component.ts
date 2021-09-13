import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import { MatTableDataSource} from '@angular/material/table';
import { MatPaginator, PageEvent} from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Country } from './country';

@Component({
    selector: 'country',
    templateUrl: 'countries.component.html',
    styleUrls:['./countries.component.css']
})

export class CountriesComponent implements OnInit {
    public displayColumns:string[]=['id', 'name', 'iso2', 'iso3', 'totCities'];
    public countries: MatTableDataSource<Country>;

    defaultPageIndex: number=0;
    defaultPageSize: number=10;
    public defaultSortColumn: string="name";
    public defaultSortOrder: string="asc";
    defaultFilterColumn: string="name";
    filterQuery: string=null;

    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;

    filterTextChanged: Subject<string> = new Subject<string>();

    constructor(private http:HttpClient, 
        @Inject('BASE_URL') private baseUrl:string){ }

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
         var url=this.baseUrl + 'api/Countries/'+event.pageIndex+'/'+
                 event.pageSize+'/'+ sortColumn +'/'+ sortOrder;

        if(this.filterQuery)
            url=url + '/' + this.defaultFilterColumn + '/' + this.filterQuery;

         this.http.get<any>(url)//, {params})
         .subscribe(result=>{
             this.paginator.length=result.totalCount;
             this.paginator.pageIndex=result.pageIndex;
             this.paginator.pageSize=result.pageSize;
             this.countries=new MatTableDataSource<Country>(result.data);
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
