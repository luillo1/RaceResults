import React from "react";
import ReactDatePicker, {
  ReactDatePickerCustomHeaderProps,
  ReactDatePickerProps,
} from "react-datepicker";
import { Button } from "semantic-ui-react";

const DatePickerInput = (
  props: Omit<ReactDatePickerProps, "todayButton" | "renderCustomHeader">
) => {
  return (
    <ReactDatePicker
      {...props}
      todayButton={
        <div style={{ marginLeft: "5px", marginRight: "5px" }}>
          <Button
            primary
            fluid
            onClick={(event: React.MouseEvent<HTMLButtonElement>) => {
              event.preventDefault();
            }}
            content="Today"
            size={"tiny"}
          />
        </div>
      }
      renderCustomHeader={({
        date,
        decreaseMonth,
        increaseMonth,
        prevMonthButtonDisabled,
        nextMonthButtonDisabled,
      }: ReactDatePickerCustomHeaderProps) => (
        <div
          style={{
            margin: 10,
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            fontSize: "1rem",
          }}
        >
          <Button
            basic
            onClick={(event: React.MouseEvent<HTMLButtonElement>) => {
              event.preventDefault();
              decreaseMonth();
            }}
            disabled={prevMonthButtonDisabled}
            icon="angle left"
            size={"tiny"}
          />

          <span>
            {date.toLocaleString("default", { month: "long", year: "numeric" })}
          </span>

          <Button
            basic
            onClick={(event: React.MouseEvent<HTMLButtonElement>) => {
              event.preventDefault();
              increaseMonth();
            }}
            disabled={nextMonthButtonDisabled}
            icon="angle right"
            size={"tiny"}
          />
        </div>
      )}
    />
  );
};

export default DatePickerInput;
