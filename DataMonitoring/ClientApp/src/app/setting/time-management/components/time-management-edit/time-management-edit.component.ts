import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { TimeManagementService } from "../../time-management-service";
import { I18nService } from "@app/shared/i18n/i18n.service";
import { TimeManagementForm, TimeManagement, TimeRange, SlipperyTime } from "../../time-management";

@Component( {
    selector: 'app-time-management-edit',
    templateUrl: './time-management-edit.component.html'
} )
export class TimeManagementEditComponent implements OnInit {

    public timeManagementFormGroup: FormGroup;
    public errorMessage: string;
    public isAddMode: boolean;
    public titleView: string;
    public timeManagementForm: TimeManagementForm;
    public timeManagement: TimeManagement;
    public timeManagementTypes: any[];
    public selectedTimeManagementType: number;
    public unitOfTimes: any[];


    constructor(
        private route: ActivatedRoute,
        private formBuilder: FormBuilder,
        private timeManagementService: TimeManagementService,
        private i18nService: I18nService,
        private router: Router ) {
    }

    ngOnInit() {

        this.timeManagementService.getTimeManagementTypes()
            .subscribe( result => {
                this.timeManagementTypes = result;
            }, error => {
                this.errorMessage = error;
            } );

        this.timeManagementService.getUnitOfTimes()
            .subscribe( result => {
                this.unitOfTimes = result;
            }, error => {
                this.errorMessage = error;
            } );

        const id = this.route.snapshot.params['id'];
        if ( id != null ) {
            // mode edition :
            this.isAddMode = false;
            this.titleView = this.i18nService.getTranslation( 'Edit' );
            this.timeManagementService.getById( +id )
                .subscribe( result => {
                    this.timeManagement = result;
                    this.initForm();
                }, error => {
                    this.errorMessage = error;
                } );
        } else {
            // mode ajout :
            this.isAddMode = true;
            this.titleView = this.i18nService.getTranslation( 'Add' );
            this.initForm();
        }

    }

    initForm() {
        this.timeManagementFormGroup = this.formBuilder.group( {
            name: [this.timeManagement != null ? this.timeManagement.name : '', Validators.required],
            unitOfTime: [this.timeManagement != null && this.timeManagement.slipperyTime != null
                ? this.timeManagement.slipperyTime.unitOfTime
                : 0],
            timeBack: [this.timeManagement != null && this.timeManagement.slipperyTime != null
                ? this.timeManagement.slipperyTime.timeBack
                : '1'],
            timeRanges: this.formBuilder.array( [] ),
        } );
        if ( this.timeManagement != null && this.timeManagement.timeRanges != null ) {
            this.initializeTimeRangesGroup( this.timeManagement.timeRanges );
        }

        if ( this.timeManagement != null ) {
            if ( this.timeManagement.slipperyTime != null ) {
                this.selectedTimeManagementType = 0;
            } else {
                this.selectedTimeManagementType = 1;
            }
        }
    }

    onResetForm() {
        const timeRanges = this.getTimeRangesArray();
        while ( timeRanges.length ) {
            timeRanges.removeAt( 0 );
        }
        this.selectedTimeManagementType = null;
        this.timeManagementFormGroup.reset();
    }

    onSubmitForm() {
        this.errorMessage = null;
        const formGroupValue = this.timeManagementFormGroup.value;

        this.timeManagementForm = formGroupValue;

        if ( this.timeManagement == null ) {
            this.timeManagement = new TimeManagement();
        }

        this.timeManagement.name = this.timeManagementForm.name;
        if ( this.selectedTimeManagementType == 0 ) {
            this.timeManagement.slipperyTime = new SlipperyTime();
            this.timeManagement.slipperyTime.timeBack = this.timeManagementForm.timeBack;
            this.timeManagement.slipperyTime.unitOfTime = this.timeManagementForm.unitOfTime;
            this.timeManagement.timeRanges = null;
        } else {

            this.timeManagement.slipperyTime = null;

            this.timeManagement.timeRanges = new Array<TimeRange>();

            this.timeManagementForm.timeRanges.forEach( element => {
                const timeRange = new TimeRange();
                timeRange.name = element.name;
                timeRange.startTimeUtc = this.timeManagementService.getUtcDate( element.startTime );
                if ( element.endTimeDisabled ) {
                    timeRange.endTimeUtc = null;
                } else {
                    timeRange.endTimeUtc = this.timeManagementService.getUtcDate( element.endTime );
                }
                this.timeManagement.timeRanges.push( timeRange );
            } );
        }

        this.updateOrAddTimeManagement( this.timeManagement );
    }

    private updateOrAddTimeManagement( timeManagement: TimeManagement ) {
        console.log( timeManagement );
        this.timeManagementService.post( timeManagement )
            .subscribe( result => {
                this.router.navigate( ['/time/times'] );
            }, error => {
                this.errorMessage = error;
            } );
    }

    onChangeTypeSelection() {
        this.errorMessage = null;
        if ( this.selectedTimeManagementType == 0 ) {
            // Slippery Time : temps glissant
            this.timeManagementFormGroup.patchValue( {
                unitOfTime: this.unitOfTimes[0].value,
                timeBack: 1,
            } );
        } else {
            // Range Time : plage de temps
        }
    }

    //////////////////////////////////////////////////////////////////////
    // TIME RANGES :
    //////////////////////////////////////////////////////////////////////
    createEmptyTimeRangeGroup() {
        return this.formBuilder.group( {
            name: [''],
            startTime: ['08:00'],
            endTime: ['17:00'],
            endTimeDisabled: false,
        } );
    }

    initializeTimeRangesGroup( timeRanges: TimeRange[] ) {
        const array = this.getTimeRangesArray();
        timeRanges.forEach( element => {
            array.push( this.createTimeRangeGroup( element ) );
        } );
    }

    createTimeRangeGroup( element: TimeRange ) {
        const timeRange = this.formBuilder.group( {
            name: [element.name],
            startTime: [this.timeManagementService.getFormatedHour( element.startTimeUtc )],
            endTime: [element.endTimeUtc != null ? this.timeManagementService.getFormatedHour( element.endTimeUtc ) : ''],
            endTimeDisabled: element.endTimeUtc == null ? true : false,
        } );

        return timeRange;
    }

    getTimeRangesArray(): FormArray {
        return this.timeManagementFormGroup.get( 'timeRanges' ) as FormArray;
    }

    onAddTimeRange() {
        const array = this.getTimeRangesArray();
        array.push( this.createEmptyTimeRangeGroup() );
    }

    onRemoveTimeRange( i: number ) {
        const control = this.getTimeRangesArray();
        control.removeAt( i );
    }
}
