import { Component, OnInit, ViewChild } from '@angular/core';
import {DynamicFormComponent} from "@app/shared/form/dynamic-form/dynamic-form.component";
import {Color} from "@app/shared/models/color";
import {ActivatedRoute, Router} from "@angular/router";
import {I18nService} from "@app/shared/i18n/i18n.service";
import {ColorService} from "../../color.service";
import {Validators} from "@angular/forms";
import {FieldConfig} from "@app/shared/form/dynamic-form/field.interface";

@Component({
  selector: 'app-color-edit',
  templateUrl: './color-edit.component.html',
})
export class ColorEditComponent implements OnInit {

  @ViewChild(DynamicFormComponent) form: DynamicFormComponent;

  public regConfig: FieldConfig[] = null;
  regConfigInit: FieldConfig[] = [
    {
      type: "backButton",
      route: "/color/colors",
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
      type: "input",
      label: this.i18nService.getTranslation('TxtClassName'),
      inputType: "text",
      name: "txtClassName",
      validations: [
        //{
        //  name: "required",
        //  validator: Validators.required,
        //  message: this.i18nService.getTranslation('Required')
        //},
        {
          name: "maxLenght",
          validator: Validators.maxLength(100),
          message: this.i18nService.getTranslation('MaxLenght')
        }
      ]
    },
    {
      type: "input",
      label: this.i18nService.getTranslation('BgClassName'),
      inputType: "text",
      name: "bgClassName",
      validations: [
        {
          name: "maxLenght",
          validator: Validators.maxLength(100),
          message: this.i18nService.getTranslation('MaxLenght')
        }
      ]
    },
    {
      type: "input",
      label: this.i18nService.getTranslation('HexColorCode'),
      inputType: "text",
      name: "hexColorCode",
      validations: [
        {
          name: "maxLenght",
          validator: Validators.maxLength(7),
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
  public color: Color;
  
  constructor(private route: ActivatedRoute, private i18nService: I18nService, private colorService: ColorService, private router: Router) { }

  ngOnInit() {

    // l'objet snapshot contient les paramètres de l'URL
    const id = this.route.snapshot.params['id'];

    if (id != null) {
      // mode edition :
      this.titleView = this.i18nService.getTranslation('Edit');
      this.colorService.getById(+id)
        .subscribe(result => {
          this.color = result;
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
    itemName.value = this.color.name;

    const itemTxtClassName = this.regConfigInit.find(x => x.name == "txtClassName");
    itemTxtClassName.value = this.color.txtClassName;

    const itemBgClassName = this.regConfigInit.find(x => x.name == "bgClassName");
    itemBgClassName.value = this.color.bgClassName;

    const itemHexColorCode = this.regConfigInit.find(x => x.name == "hexColorCode");
    itemHexColorCode.value = this.color.hexColorCode;

    this.regConfig = this.regConfigInit;
  }

  submit(value: any) {
    const formColorValue = <Color>value;

    if (this.color == null) {
      this.color = new Color();
    }

    this.color.name = formColorValue.name;
    this.color.txtClassName = formColorValue.txtClassName;
    this.color.bgClassName = formColorValue.bgClassName;
    this.color.hexColorCode = formColorValue.hexColorCode;
    
    if (this.color.id == 0) {
      this.colorService.post(this.color)
        .subscribe(result => {
          this.goBack();
        }, error => {
          this.errorMessage = error;
        });
    } else {
      this.colorService.put(this.color.id, this.color)
        .subscribe(result => {
          this.goBack();
        }, error => {
          this.errorMessage = error;
        });
    }
  }

  goBack() {
    this.router.navigate(['/color/colors']);
  }

}
