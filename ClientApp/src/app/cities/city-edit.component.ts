import { Component, Inject, OnInit } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import { ActivatedRoute, Router} from '@angular/router';
import { FormGroup, FormControl, Validators, AbstractControl, AsyncValidatorFn} from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { BaseFormComponent } from '../base.form.component';

import {City} from './city';
import { Country } from '../countries/country';

import { CityService } from './city.service';
import { ApiResult } from '../base.service';

@Component({
    selector: 'cityEdit',
    templateUrl: 'city-edit.component.html',
    styleUrls:['./city-edit.component.css']
})

export class CityEditComponent extends BaseFormComponent
         implements OnInit {
    //the view Title
    title: string;

    //the form model
    form: FormGroup;

    //the city object to edit or create
    city: City;

    id?: number;

    countries: Country[];

    constructor(private activatedRoute: ActivatedRoute, private router:Router,
        private http: HttpClient, private cityService: CityService) {
            //@Inject('BASE_URL') private baseUrl: string
            super();
        }

    ngOnInit(){
        this.form=new FormGroup({
            name: new FormControl('', Validators.required),
            lat: new FormControl('', [Validators.required,
                 Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)]),
            lon: new FormControl('', [Validators.required,
                 Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)]),
            countryId: new FormControl('', Validators.required)
        }, null, this.isDupeCity());

        this.loadData();
    }

    isDupeCity(): AsyncValidatorFn {
        return (control: AbstractControl): Observable<{[key: string]: any} | null> => {
            var city=<City>{};
            city.id= (this.id) ? this.id : 0;
            city.name = this.form.get("name").value;
            city.lat = this.form.get("lat").value;
            city.lon = this.form.get("lon").value;
            city.countryId = this.form.get("countryId").value;
            
            //var url=this.baseUrl + "api/Cities/IsDupeCity";
            return this.cityService.isDupeCity(city).pipe(map(res=>{
                return (res ? { isDupeCity: true} : null)
            }));
        }
    }

    loadData(){
        //load countries
        this.loadCountries();

        //retrieve the id from the 'id' from parameter
        this.id= +this.activatedRoute.snapshot.paramMap.get('id');
        if(this.id){
            //edit mode
            //featch the city from the server
            //var url=this.baseUrl + "api/Cities/" + this.id;
            this.cityService.get<City>(this.id).subscribe(result=>{
                this.city=result;
                this.title="Edit - "+this.city.name;

                //update the form with the city value
                this.form.patchValue(this.city);
            }, error=>console.error(error));
        }else{
            //add new mode
            this.title='Create new City';
        }
    }

    loadCountries(){
        //featch all countries from server
        //var url=this.baseUrl + 'api/Countries/0/9999/name/';
        this.cityService.getCountries<ApiResult<Country>>(0,9999,"name",null,null,null).subscribe(result=>{
            this.countries = result.data;
        }, error=> console.error(error));
    }

    onSubmit(){
        var city=(this.id)?  this.city: <City>{};
        city.name= this.form.get("name").value;
        city.lat= this.form.get("lat").value;
        city.lon= this.form.get("lon").value;
        city.countryId= this.form.get("countryId").value;

        if(this.id){
            //edit mode
            //var url=this.baseUrl + "api/Cities/" + this.city.id;
            this.cityService.put<City>(city).subscribe(res=>{
                console.log("City "+ city.id + "has been updated.");

                //go back to cities view
                this.router.navigate(['/cities']);
            }, error=> console.error(error));
        }else{
            //add mode
            //var url=this.baseUrl + "api/Cities";
            this.cityService.post<City>(city).subscribe(result=>{
                console.log("City "+ result.id + "has been created.");

                //go back to cities view
                this.router.navigate(['/cities']);
            }, error=> console.error(error));
        }
    }
}