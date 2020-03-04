import { Injectable } from '@angular/core';

import {BaseService} from "../base.service";
import {JsonApiService} from "@app/core/services";
import {Style} from "@app/shared/models/style";

@Injectable({
  providedIn: 'root'
})
export class StyleService extends BaseService<Style> {

  constructor(jsonApiService: JsonApiService) {
    super(jsonApiService, 'Style');
  }

}

