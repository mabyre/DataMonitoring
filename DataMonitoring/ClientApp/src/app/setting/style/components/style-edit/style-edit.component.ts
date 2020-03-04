import { Component, OnInit, ViewChild } from '@angular/core';
import {DynamicFormComponent} from "@app/shared/form/dynamic-form/dynamic-form.component";
import {FieldConfig} from "@app/shared/form/dynamic-form/field.interface";
import {Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {I18nService} from "@app/shared/i18n/i18n.service";
import {StyleService} from "../../style.service";
import {Style} from "@app/shared/models/style";

@Component({
  selector: 'app-style-edit',
  templateUrl: './style-edit.component.html'
})
export class StyleEditComponent implements OnInit {

  @ViewChild(DynamicFormComponent) form: DynamicFormComponent;

  public regConfig: FieldConfig[] = null;
  regConfigInit: FieldConfig[] = [
    {
      type: "backButton",
      route: "/style/styles",
      label: this.i18nService.getTranslation('Back')
    },
    {
      type: "input",
      label: this.i18nService.getTranslation('Name'),
      inputType: "text",
      name: "name",
      validations: [
        {
          name: "required",
          validator: Validators.required,
          message: this.i18nService.getTranslation('Required')
        },
        {
          name: "maxLenght",
          validator: Validators.maxLength(60),
          message: this.i18nService.getTranslation('MaxLenght')
        }
      ]
    },
    {
      type: "textarea",
      label: this.i18nService.getTranslation('Code'),
      name: "code",
      validations: [
        {
          name: "required",
          validator: Validators.required,
          message: this.i18nService.getTranslation('Required')
        },
        {
          name: "maxLenght",
          validator: Validators.maxLength(500),
          message: this.i18nService.getTranslation('MaxLenght')
        }
      ]
    },
    {
      type: "submitButton",
      label: this.i18nService.getTranslation('Save')
    },
  ];

  public errorMessage: string;
  public titleView: string;
  public style: Style;

  constructor(private route: ActivatedRoute, private i18nService: I18nService, private styleService: StyleService, private router: Router) { }

  ngOnInit() {
    // l'objet snapshot contient les paramètres de l'URL
    const id = this.route.snapshot.params['id'];

    if (id != null) {
      // mode edition :
      this.titleView = this.i18nService.getTranslation('Edit');
      this.styleService.getById(+id)
        .subscribe(result => {
          this.style = result;
          this.editInit();
        }, error => {
          this.errorMessage = error;
        });
    } else {
      // mode ajout :
      this.titleView = this.i18nService.getTranslation('Add');
      this.regConfig = this.regConfigInit;
    }
  }

  editInit(): any {
    const itemName = this.regConfigInit.find(x => x.name == "name");
    itemName.value = this.style.name;

    const itemCode = this.regConfigInit.find(x => x.name == "code");
    itemCode.value = this.style.code;

    this.regConfig = this.regConfigInit;
  }

  submit(value: any) {
    const formStyleValue = <Style>value;

    if (this.style == null) {
      this.style = new Style();
    }

    this.style.name = formStyleValue.name;
    this.style.code = formStyleValue.code;
    
    if (this.style.id == 0) {
      this.styleService.post(this.style)
        .subscribe(result => {
          this.goBack();
        }, error => {
          this.errorMessage = error;
        });
    } else {
      this.styleService.put(this.style.id, this.style)
        .subscribe(result => {
          this.goBack();
        }, error => {
          this.errorMessage = error;
        });
    }
  }

  goBack() {
    this.router.navigate(['/style/styles']);
  }

  
}


