//TypeScriptコンパイラ用型定義
declare class SchedulePattern extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
    fetch(info_id: Number, options: any): void;
    getDataSource(data: any): kendo.data.DataSource;
    getLineById(line_id: number): any;
}

define(['models/schedule_pattern_line', 'models/schedule_pattern_item'],
    function (SchedulePatternLine, SchedulePatternItem) {

        var SchedulePattern = <typeof SchedulePattern>kendo.data.Model.define({
            id: "sch_pattern_id",
            fields: {
                "sch_pattern_id": { type: "number" },
                "description": { type: "string" },
                "last_person": { type: "string" },
                "update_date": { type: "date" }
            }
        });

        SchedulePattern.prototype.parseJSON = function(){
            this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
            this.update_date = App.Utils.convertToDateTime(this.update_date);

            var _lines = this.get("Lines");
            if (_lines) {
                var newLines = [];
                $.each(_lines, function (index, item) {
                    var newItem = new SchedulePatternLine(item);
                    newItem.parseJSON();
                    newLines.push(newItem);
                });
                this.set("Lines", newLines);
            }

            this.dirty = false;
        };

        SchedulePattern.prototype.getLineById = function(line_id){
        var len = this.Lines.length;
        for (var i = 0; i < len; i++) {
            if (this.Lines[i].get("sch_param_line_id") === line_id) {
                return this.Lines[i];
            }
        }
        return null;
    };

    SchedulePattern.prototype.save = function(options){
        var dataStr = JSON.stringify(this);

        $.ajax({
            url: App.getApiPath("SchedulePatterns/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        })
            .done(function (result) {
                if (options && options.success) options.success(result);
            }).fail(function (jqXHR, textStatus, errorThrown) {
                if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
            }).always(function () {
                if (options && options.always) options.always();
            });
    };

    SchedulePattern.prototype.destroy = function(options){
        $.ajax({
            url: App.getApiPath("SchedulePatterns/Delete/" + this.get("sch_pattern_id")),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        })
            .done(function (result) {
                if (options && options.success) options.success(result);
            }).fail(function (jqXHR, textStatus, errorThrown) {
                if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
            }).always(function () {
                if (options && options.always) options.always();
            });
    };

        SchedulePattern.fetch = function (sch_pattern_id, options) {
            $.ajax({
                url: App.getApiPath("SchedulePatterns/" + sch_pattern_id),
                type: "GET",
                cache: false
            })
                .done(function (result) {
                    var data = new SchedulePattern(result);
                    data.parseJSON();
                    data.dirty = false;

                    if (options && options.success) options.success(data);
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
                }).always(function () {
                    if (options && options.always) options.always();
                });
        };

        SchedulePattern.getDataSource = function (data) {
            return new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("SchedulePatterns/Search"),
                        dataType: "json",
                        data: data
                    }
                },
                schema: { model: SchedulePattern }
            });
        };

        return SchedulePattern;
    }
);

