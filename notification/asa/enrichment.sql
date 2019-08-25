SELECT
    *,
    GetMetadataPropertyValue(iothub, '[User].reported_property_name') as reported_message_field_name,
    GetMetadataPropertyValue(iothub, '[User].desired_property_name') as desired_message_field_name
INTO
    [blob]
FROM
    [iothub]