WITH SelectPreviousEvent AS
(
    SELECT
        [time],
        [temperature],
        iothub.IoTHub.ConnectionDeviceId as DeviceId,
        LAG([time]) OVER (LIMIT DURATION(hour, 24)) as previousTime,
        LAG([temperature]) OVER (LIMIT DURATION(hour, 24)) as previousTemperature
    FROM iothub TIMESTAMP BY [time]
)
SELECT
    DeviceId,
    LAG([time]) OVER (LIMIT DURATION(hour, 24) WHEN previousTemperature < 50) as FAULT_START,
    previousTime as FAULT_END,
    DATEDIFF(second, 
        LAG([time]) OVER (LIMIT DURATION(hour, 24) WHEN previousTemperature < 50),
        previousTime) as fault_duration
into notifications
FROM SelectPreviousEvent
WHERE
    ([temperature] < 50
    AND previousTemperature >= 50)
    AND DATEDIFF(second, 
        LAG([time]) OVER (LIMIT DURATION(hour, 24) WHEN previousTemperature < 50),
        previousTime) >= 5

