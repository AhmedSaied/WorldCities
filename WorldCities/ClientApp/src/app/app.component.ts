import { Component } from '@angular/core';
import { ConnectionService } from 'ng-connection-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';
  
  status='ONLINE';
  isConnected=true;
  constructor(private connectionService: ConnectionService){
    this.connectionService.monitor().subscribe(isConnect=>{
      this.isConnected=isConnect;
      if(this.isConnected){
        this.status="ONLINE";
      }else{
        this.status="OFFLINE";
      }
    })
  }
}
