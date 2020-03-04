import { Injectable } from '@angular/core';
import {Color} from "@app/shared/models/color";
import {BaseService} from "../base.service";
import {JsonApiService} from "@app/core/services";

@Injectable({
  providedIn: 'root'
})
export class ColorService extends BaseService<Color> {

  constructor(jsonApiService: JsonApiService) {
    super(jsonApiService, 'Color');
  }
}
