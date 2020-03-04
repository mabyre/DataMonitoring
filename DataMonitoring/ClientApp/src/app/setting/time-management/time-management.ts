export class TimeManagement {

    public id: number;
    public name: string;

    public slipperyTime: SlipperyTime;
    public timeRanges: TimeRange[];

    public typeToDisplay: string;
}

export class TimeManagementForm {

    public id: number;
    public name: string;

    // Slippery Time
    public unitOfTime: string;
    public timeBack: number;

    // Time Range
    public timeRanges: TimeRangeLocal[];
}

export class SlipperyTime {

    public unitOfTime: string;
    public timeBack: number;
}

export class TimeRangeLocal {
    public name: string;
    public startTime: string;
    public endTime: string;
    public endTimeDisabled: boolean;
}

export class TimeRange {

    public name: string;
    public startTimeUtc: Date;
    public endTimeUtc: Date;
    public endTimeDisabled: boolean;
}
