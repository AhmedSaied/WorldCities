import { Component, Inject, OnInit } from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import { ActivatedRoute, Router} from '@angular/router';
import { FormGroup, FormBuilder, Validators, AbstractControl, AsyncValidatorFn} from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { Country } from './country';

import { BaseFormComponent } from '../base.form.component';

@Component({
    selector: 'countryEdit',
    templateUrl: 'country-edit.component.html',
    styleUrls:['./country-edit.component.css']
})

export class CountryEditComponent extends BaseFormComponent
        implements OnInit {
        //the view Title
        title: string;

        //the form model
        form: FormGroup;
    
        //the country object to edit or create
        country: Country;
    
        id?: number;

        constructor(private fb: FormBuilder, private activatedRoute: ActivatedRoute,
             private router:Router,private http: HttpClient, 
             @Inject('BASE_URL') private baseUrl: string) {
                //this.loadData();
                super();
            }

        ngOnInit(){
            this.form= this.fb.group({
                name: ['', [Validators.required, this.isDupeField("name")]],
                iso2: ['', [Validators.required, Validators.pattern(/^[a-zA-Z]{2}$/), this.isDupeField("iso2")]],
                iso3: ['', [Validators.required, Validators.pattern(/^[a-zA-Z]{3}$/), this.isDupeField("iso3")]],
            });
            this.loadData();
        }

    loadData(){
        //retrieve the id from the 'id' from parameter
        this.id= +this.activatedRoute.snapshot.paramMap.get('id');
        if(this.id){
            //edit mode
            //featch the country from the server
            var url=this.baseUrl + "api/Countries/" + this.id;
            this.http.get<Country>(url).subscribe(result=>{
                this.country=result;

                this.title="Edit - "+this.country.name;

                //update the form with the country value
                this.form.patchValue(this.country);
            }, err=>console.error(err));
        }else{
            //add new mode
            this.title='Create new Country';
        }
    }

    isDupeField(fieldName: string): AsyncValidatorFn {
        return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {
    
          /*var params = new HttpParams()
            .set("countryId", (this.id) ? this.id.toString() : "0")
            .set("fieldName", fieldName)
            .set("fieldValue", control.value);*/
            var countryId=(this.id) ? this.id.toString() : "0";
          var url = this.baseUrl + "api/Countries/IsDupeField/"+countryId+"/"+fieldName+"/"+control.value;
          return this.http.post<boolean>(url, null)
            .pipe(map(result => {
                console.log(result);
              return (result ? { isDupeField: true } : null);
          }));
        }
    }

    onSubmit(){
        var country=(this.id)?  this.country: <Country>{};
        country.name= this.form.get("name").value;
        country.iso2= this.form.get("iso2").value;
        country.iso3= this.form.get("iso3").value;

        if(this.id){
            //edit mode
            var url=this.baseUrl + "api/Countries/" + this.country.id;
            this.http.put<Country>(url, country).subscribe(res=>{
                console.log("Country "+ country.id + "has been updated.");

                //go back to countries view
                this.router.navigate(['/countries']);
            }, error=> console.error(error));
        }else{
            //add mode
            var url=this.baseUrl + "api/Countries";
            this.http.post<Country>(url, country).subscribe(result=>{
                console.log("Country "+ result.id + "has been created.");

                //go back to countries view
                this.router.navigate(['/countries']);
            }, error=> console.error(error));
        }
    }
}