#include <stddef.h>
#include <stdio.h>
#include <stdint.h>
#include <stdbool.h>
#include <rawfile/raw_file.h>
#include <native_buffer/native_buffer.h>
#include <IPCKit/ipc_cremote_object.h>
#include <arkui/native_interface_accessibility.h>

int main(void)
{
    printf("{\"schemaVersion\":1,\"rawFileDescriptor\":{\"size\":%zu,\"align\":%zu,\"fdOffset\":%zu,\"startOffset\":%zu,\"lengthOffset\":%zu},\n",
        sizeof(RawFileDescriptor), _Alignof(RawFileDescriptor), offsetof(RawFileDescriptor, fd),
        offsetof(RawFileDescriptor, start), offsetof(RawFileDescriptor, length));
    printf("\"nativeBufferConfig\":{\"size\":%zu,\"align\":%zu,\"strideOffset\":%zu},\n",
        sizeof(OH_NativeBuffer_Config), _Alignof(OH_NativeBuffer_Config), offsetof(OH_NativeBuffer_Config, stride));
    printf("\"nativeBufferPlane\":{\"size\":%zu,\"align\":%zu,\"rowStrideOffset\":%zu},\n",
        sizeof(OH_NativeBuffer_Plane), _Alignof(OH_NativeBuffer_Plane), offsetof(OH_NativeBuffer_Plane, rowStride));
    printf("\"accessibleRect\":{\"size\":%zu,\"align\":%zu,\"rightBottomXOffset\":%zu},\n",
        sizeof(ArkUI_AccessibleRect), _Alignof(ArkUI_AccessibleRect), offsetof(ArkUI_AccessibleRect, rightBottomX));
    printf("\"accessibleRange\":{\"size\":%zu,\"align\":%zu,\"currentOffset\":%zu},\n",
        sizeof(ArkUI_AccessibleRangeInfo), _Alignof(ArkUI_AccessibleRangeInfo), offsetof(ArkUI_AccessibleRangeInfo, current));
    printf("\"accessibleGrid\":{\"size\":%zu,\"align\":%zu,\"selectionModeOffset\":%zu},\n",
        sizeof(ArkUI_AccessibleGridInfo), _Alignof(ArkUI_AccessibleGridInfo), offsetof(ArkUI_AccessibleGridInfo, selectionMode));
    printf("\"accessibleGridItem\":{\"size\":%zu,\"align\":%zu,\"columnIndexOffset\":%zu}\n",
        sizeof(ArkUI_AccessibleGridItemInfo), _Alignof(ArkUI_AccessibleGridItemInfo), offsetof(ArkUI_AccessibleGridItemInfo, columnIndex));
    printf("}\n");
    return 0;
}
