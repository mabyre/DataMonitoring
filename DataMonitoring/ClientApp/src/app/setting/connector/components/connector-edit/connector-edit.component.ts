import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

import { NgxSpinnerService } from 'ngx-spinner';

import { ConnectorService } from '../../connector.service';
import { I18nService } from "@app/shared/i18n/i18n.service";
import { TimeZoneService } from "@app/core/services";
import { TimeZone } from '@app/core/timeZone';
import { Connector, SqlServerConnector, ApiConnector, SqLiteConnector } from "../../connector";

@Component({
  selector: 'app-connector-edit',
  templateUrl: './connector-edit.component.html'
})
export class ConnectorEditComponent implements OnInit {

  public connector: Connector;
  public connectorApiForm: FormGroup;
  public connectorSqlForm: FormGroup;
  public connectorSqLiteForm: FormGroup;
  public grantTypes: any[];
  public connectorTypes: any[];
  public autorisationTypes: any[];
  public httpMethods: any[];
  public titleView: string;

  public timeZones: TimeZone[];
  public currentServerTimezone: string;

  public errorMessage: string;

  public selectedConnectorType: number;
  public testConnexionOk: boolean = null;
  public isAddMode: boolean;

  constructor(private formBuilder: FormBuilder, private connectorsService: ConnectorService,
      private route: ActivatedRoute, private router: Router, private i18nService: I18nService,
      private spinner: NgxSpinnerService, private timeZoneService: TimeZoneService) {
      this.httpMethods = connectorsService.getHttpMethods();
  }

  ngOnInit() {

      this.timeZoneService.getTimeZones()
          .subscribe(result => {
              this.timeZones = result;
          }, error => {
              this.errorMessage = error;
          });

      this.connectorsService.getConnectorTypes()
          .subscribe(result => {
              this.connectorTypes = result;
          }, error => {
              this.errorMessage = error;
          });

      this.connectorsService.getApiAutorisationTypes()
          .subscribe(result => {
              this.autorisationTypes = result;
          }, error => {
              this.errorMessage = error;
          });

      this.connectorsService.getApiGrantTypes()
          .subscribe(result => {
              this.grantTypes = result;
          }, error => {
              this.errorMessage = error;
          });

      // l'objet snapshot contient les paramètres de l'URL
      const id = this.route.snapshot.params['id'];

      if (id != null) {
          // mode edition :
          this.isAddMode = false;
          this.titleView = this.i18nService.getTranslation('Edit');
          this.connectorsService.getById(+id)
              .subscribe(result => {
                  this.connector = result;
                  this.initCurrentTimeZoneAndTypeForm();
              }, error => {
                  this.errorMessage = error;
              });
      } else {
          // mode ajout :
          this.isAddMode = true;
          this.titleView = this.i18nService.getTranslation('Add');
          this.initCurrentTimeZoneAndTypeForm();
      }
  }

    initCurrentTimeZoneAndTypeForm() {

        this.timeZoneService.getCurrentTimeZone()
            .subscribe(result => {
                this.currentServerTimezone = result.id;
                if (this.connector.apiConnector != null) {
                    this.initApiForm();
                }
                else if (this.connector.sqlServerConnector != null) {
                    this.initSqlForm();
                }
                else if (this.connector.sqLiteConnector != null) {
                    this.initSqLiteForm();
                }
            }, error => {
                this.errorMessage = error;
            });
    }

    initSqLiteForm() {

        let timezone = this.currentServerTimezone;

        if (this.connector != null && this.connector.timeZone != null) {
            timezone = this.connector.timeZone;
        }

        this.connectorApiForm = null;
        this.connectorSqlForm = null;
        this.connectorSqLiteForm = this.formBuilder.group({
            name: [this.connector != null ? this.connector.name : '', Validators.required],
            timeZone: [timezone, Validators.required],
            hostName: [this.connector != null ? this.connector.sqLiteConnector.hostName : '', Validators.required],
            databaseName: [this.connector != null ? this.connector.sqLiteConnector.databaseName : '', Validators.required],
            integratedSecurity: [this.connector != null ? this.connector.sqLiteConnector.useIntegratedSecurity : 'true', Validators.required],
            userName: [this.connector != null ? this.connector.sqLiteConnector.userName : ''],
            password: [this.connector != null ? this.connector.sqLiteConnector.password : ''],
        });
    }

    initSqlForm() {

        let timezone = this.currentServerTimezone;

        if (this.connector != null && this.connector.timeZone != null) {
            timezone = this.connector.timeZone;
        }

        this.connectorApiForm = null;
        this.connectorSqlForm = this.formBuilder.group({
            name: [this.connector != null ? this.connector.name : '', Validators.required],
            timeZone: [timezone, Validators.required],
            hostName: [this.connector != null ? this.connector.sqlServerConnector.hostName : '', Validators.required],
            databaseName: [this.connector != null ? this.connector.sqlServerConnector.databaseName : '', Validators.required],
            integratedSecurity: [this.connector != null ? this.connector.sqlServerConnector.useIntegratedSecurity : 'true', Validators.required],
            userName: [this.connector != null ? this.connector.sqlServerConnector.userName : ''],
            password: [this.connector != null ? this.connector.sqlServerConnector.password : ''],
        });
    }

    initApiForm() {

        let timezone = this.currentServerTimezone;

        if (this.connector != null && this.connector.timeZone != '')
        {
          timezone = this.connector.timeZone;
        }

        this.connectorSqlForm = null;
        this.connectorApiForm = this.formBuilder.group({
          name: [this.connector != null ? this.connector.name : '', Validators.required],
          timeZone: [timezone, Validators.required],
          baseUrl: [
            this.connector != null
              ? this.connector.apiConnector.baseUrl
              : '',
            Validators.required
          ],
          acceptHeader: [
            this.connector != null
              ? this.connector.apiConnector.acceptHeader
              : '',
            [Validators.required, Validators.pattern(/^.{1,49}$/)]
          ],
          httpMethod: [
            this.connector != null
              ? this.connector.apiConnector.httpMethod
              : this.httpMethods[0].value,
            Validators.required
          ],
          autorisationType: [
            this.connector != null
              ? this.connector.apiConnector.autorisationType
              : this.autorisationTypes[0].value,
            Validators.required
          ],
          accessTokenUrl: [this.connector != null ? this.connector.apiConnector.accessTokenUrl : ''],
          clientId: [this.connector != null ? this.connector.apiConnector.clientId : ''],
          clientSecret: [this.connector != null ? this.connector.apiConnector.clientSecret : ''],
          grantType: [this.connector != null ? this.connector.apiConnector.grantType : this.grantTypes[0].value],
        });
    }

    onSubmitSqLiteForm() {

      this.errorMessage = null;
      const formValue = this.connectorSqLiteForm.value;

      if (this.connector != null) {
          // mode edition
          this.connector.name = formValue['name'];
          this.connector.timeZone = formValue['timeZone'];

          this.connector.sqLiteConnector.hostName = formValue['hostName'];
          this.connector.sqLiteConnector.databaseName = formValue['databaseName'];
          this.connector.sqLiteConnector.useIntegratedSecurity = formValue['integratedSecurity'];
          this.connector.sqLiteConnector.userName = formValue['userName'];
          this.connector.sqLiteConnector.password = formValue['password'];

          this.updateConnector(this.connector);

      } else {
          // mode ajout
          this.connector = new Connector();
          const newSqLiteConnector = new SqLiteConnector();
          this.connector.name = formValue['name'];
          this.connector.timeZone = formValue['timeZone'];

          newSqLiteConnector.hostName = formValue['hostName'];
          newSqLiteConnector.databaseName = formValue['databaseName'];
          newSqLiteConnector.useIntegratedSecurity = formValue['integratedSecurity'];
          newSqLiteConnector.userName = formValue['userName'];
          newSqLiteConnector.password = formValue['password'];
          this.connector.sqLiteConnector = newSqLiteConnector;

          this.addConnector(this.connector);

      }
  }

  onSubmitSqlForm() {
    this.errorMessage = null;
    const formValue = this.connectorSqlForm.value;

    if (this.connector != null) {
      // mode edition
      this.connector.name = formValue['name'];
      this.connector.timeZone = formValue['timeZone'];
      
      this.connector.sqlServerConnector.hostName = formValue['hostName'];
      this.connector.sqlServerConnector.databaseName = formValue['databaseName'];
      this.connector.sqlServerConnector.useIntegratedSecurity = formValue['integratedSecurity'];
      this.connector.sqlServerConnector.userName = formValue['userName'];
      this.connector.sqlServerConnector.password = formValue['password'];

      this.updateConnector(this.connector);

    } else {
      // mode ajout
      this.connector = new Connector();
      const newSqlServerConnector = new SqlServerConnector();
      this.connector.name = formValue['name'];
      this.connector.timeZone = formValue['timeZone'];

      newSqlServerConnector.hostName = formValue['hostName'];
      newSqlServerConnector.databaseName = formValue['databaseName'];
      newSqlServerConnector.useIntegratedSecurity = formValue['integratedSecurity'];
      newSqlServerConnector.userName = formValue['userName'];
      newSqlServerConnector.password = formValue['password'];
      this.connector.sqlServerConnector = newSqlServerConnector;

      this.addConnector(this.connector);

    }
  }

  onSubmitApiForm() {
    this.errorMessage = null;
    const formValue = this.connectorApiForm.value;

    if (this.connector != null) {
      // mode edition
      this.connector.name = formValue['name'];
      this.connector.timeZone = formValue['timeZone'];

      this.connector.apiConnector.baseUrl = formValue['baseUrl'];
      this.connector.apiConnector.acceptHeader = formValue['acceptHeader'];
      this.connector.apiConnector.autorisationType = formValue['autorisationType'];
      this.connector.apiConnector.accessTokenUrl = formValue['accessTokenUrl'];
      this.connector.apiConnector.clientId = formValue['clientId'];
      this.connector.apiConnector.clientSecret = formValue['clientSecret'];
      this.connector.apiConnector.grantType = formValue['grantType'];
      this.connector.apiConnector.httpMethod = formValue['httpMethod'];

      this.updateConnector(this.connector);

    } else {
      // mode ajout
      this.connector = new Connector();
      const newApiConnector = new ApiConnector();
      this.connector.name = formValue['name'];
      this.connector.timeZone = formValue['timeZone'];

      newApiConnector.baseUrl = formValue['baseUrl'];
      newApiConnector.acceptHeader = formValue['acceptHeader'];
      newApiConnector.autorisationType = formValue['autorisationType'];
      newApiConnector.accessTokenUrl = formValue['accessTokenUrl'];
      newApiConnector.clientId = formValue['clientId'];
      newApiConnector.clientSecret = formValue['clientSecret'];
      newApiConnector.grantType = formValue['grantType'];
      newApiConnector.httpMethod = formValue['httpMethod'];
      this.connector.apiConnector = newApiConnector;

      this.addConnector(this.connector);
    }
  }

  private addConnector(connector: Connector) {

    this.connectorsService.post(connector)
      .subscribe(result => {
        this.router.navigate(['/connector/connectors']);
      }, error => {
        this.errorMessage = error;
      });
  }

  private updateConnector(connector: Connector) {
    
    this.connectorsService.put(connector.id, connector)
      .subscribe(result => {
        this.router.navigate(['/connector/connectors']);
      }, error => {
        this.errorMessage = error;
      });
  }

  onChange() {

      let selectedType = this.selectedConnectorType;

      this.errorMessage = null;
      this.testConnexionOk = null;
      //switch (selectedType) {
      //    case 0 :
      //        this.initApiForm();
      //        break;
      //    case 1 :
      //        this.initSqlForm();
      //        break;
      //    case  2 :
      //        this.initSqLiteForm();
      //        break;
      //    default:
      //        this.initSqlForm();
      //        break;
      //}

      if (selectedType == 0) {
          this.initApiForm();
      }
      else if (selectedType == 1) {
          this.initSqlForm();
      }
      else if (selectedType == 2) {
          this.initSqLiteForm();
      }
      else {

      }
  }

  onTestConnexion() {
    this.spinner.show();

    this.errorMessage = null;
    this.testConnexionOk = null;

    const formValue = this.connectorSqlForm.value;
    const sqlConnector = new SqlServerConnector();
    sqlConnector.hostName = formValue['hostName'];
    sqlConnector.databaseName = formValue['databaseName'];
    sqlConnector.useIntegratedSecurity = formValue['integratedSecurity'];
    sqlConnector.userName = formValue['userName'];
    sqlConnector.password = formValue['password'];
    const connector = new Connector();
    connector.sqlServerConnector = sqlConnector;

    this.connectorsService.testConnection(connector)
      .subscribe(result => {
        this.spinner.hide();
        this.testConnexionOk = result;
      }, error => {
        this.testConnexionOk = false;
        this.errorMessage = error;
        this.spinner.hide();
      });
  }

}
