/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { RcaptchaserviceService } from './rcaptchaservice.service';

describe('Service: Rcaptchaservice', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [RcaptchaserviceService]
    });
  });

  it('should ...', inject([RcaptchaserviceService], (service: RcaptchaserviceService) => {
    expect(service).toBeTruthy();
  }));
});
