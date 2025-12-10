using MapGeneration;
using UnityEngine;

namespace FrikanUtils.Spawnpoints.LootSpawn;

/// <summary>
/// Utilities to help with the <see cref="LootSpawnPoint"/>
/// </summary>
public static class LootSpawnPointUtilities
{
    /// <summary>
    /// Get the associated data for a <see cref="LootPoint"/>.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static LootPointData GetPointData(this LootPoint point)
    {
        switch (point)
        {
            case LootPoint.IntercomCrate:
                return new LootPointData(RoomName.EzIntercom, new Vector3(-1.983f, -4.889f, 4.444f));
            case LootPoint.IntercomDesk:
                return new LootPointData(RoomName.EzIntercom, new Vector3(-3.71f, -4.691f, -3.832f));
            case LootPoint.IntercomDeskSide:
                return new LootPointData(RoomName.EzIntercom, new Vector3(-6.853f, -4.691f, -3.626f));
            case LootPoint.PCsDesk:
                return new LootPointData(RoomName.EzOfficeLarge, new Vector3(-5.245f, 1.134f, -4.152f));
            case LootPoint.PCsDeskAlt:
                return new LootPointData(RoomName.EzOfficeLarge, new Vector3(1.755f, 1.134f, -0.402f));
            case LootPoint.PCsShelf:
                return new LootPointData(RoomName.EzOfficeLarge, new Vector3(-2.68f, 1.134f, -7.004f));
            case LootPoint.PCsShelfAlt:
                return new LootPointData(RoomName.EzOfficeLarge, new Vector3(-3.811f, 1.134f, -7.004f));
            case LootPoint.PCsCubicle:
                return new LootPointData(RoomName.EzOfficeLarge, new Vector3(-5.629f, 1.134f, 7.003f));
            case LootPoint.PCsSmallLedge:
                return new LootPointData(RoomName.EzOfficeSmall, new Vector3(1.12f, 1.291f, 4.918f));
            case LootPoint.PCsSmallLedgeAlt:
                return new LootPointData(RoomName.EzOfficeSmall, new Vector3(1.12f, 1.291f, -5.175f));
            case LootPoint.PCsSmallDesk:
                return new LootPointData(RoomName.EzOfficeSmall, new Vector3(5.895f, -0.387f, -3.494f));
            case LootPoint.PCsSmallDeskAlt:
                return new LootPointData(RoomName.EzOfficeSmall, new Vector3(1.612f, -0.387f, -2.716f));
            case LootPoint.ShelterWorkstation:
                return new LootPointData(RoomName.EzEvacShelter, new Vector3(1.126f, 1.067f, 6.705f));
            case LootPoint.ShelterWorkstationAlt:
                return new LootPointData(RoomName.EzEvacShelter, new Vector3(2.619f, 1.067f, 6.219f));
            case LootPoint.PCsStoriedDesk:
                return new LootPointData(RoomName.EzOfficeStoried, new Vector3(-1.041f, 1.067f, -3.354f));
            case LootPoint.PCsStoriedDeskAlt:
                return new LootPointData(RoomName.EzOfficeStoried, new Vector3(-3.82f, 1.067f, 3.56f));
            case LootPoint.PCsStoriedShelf:
                return new LootPointData(RoomName.EzOfficeStoried, new Vector3(-1.806f, 1.067f, -7.016f));
            case LootPoint.PCsStoriedShelfAlt:
                return new LootPointData(RoomName.EzOfficeStoried, new Vector3(-3.871f, 1.067f, -7.05f));
            case LootPoint.PCsStoriedWorkstation:
                return new LootPointData(RoomName.EzOfficeStoried, new Vector3(6.791f, 3.939f, 6.512f));
            case LootPoint.PCsStoriedWorkstationAlt:
                return new LootPointData(RoomName.EzOfficeStoried, new Vector3(5.174f, 3.939f, 6.678f));
            case LootPoint.Scp049Spawn173:
                return new LootPointData(RoomName.Hcz049, new Vector3(22.696f, 196.982f, 12.535f));
            case LootPoint.Scp049Spawn173Outside:
                return new LootPointData(RoomName.Hcz049, new Vector3(18.067f, 196.982f, 7.886f));
            case LootPoint.Scp049Spawn:
                return new LootPointData(RoomName.Hcz049, new Vector3(-2.576f, 192.798f, -10.767f));
            case LootPoint.Scp049Armory:
                return new LootPointData(RoomName.Hcz049, new Vector3(-4.023f, 192.798f, -6.749f));
            case LootPoint.Scp049ArmoyWorkstation:
                return new LootPointData(RoomName.Hcz049, new Vector3(-5.41f, 193.41f, -4.775f));
            case LootPoint.Scp079Intermediate:
                return new LootPointData(RoomName.Hcz079, new Vector3(10.712f, -2.1f, -5.607f));
            case LootPoint.Scp079Table:
                return new LootPointData(RoomName.Hcz079, new Vector3(1.687f, -2.393f, -7.797f));
            case LootPoint.Scp079Workstation:
                return new LootPointData(RoomName.Hcz079, new Vector3(5.997f, -2.312f, -5.003f));
            case LootPoint.Scp096:
                return new LootPointData(RoomName.Hcz096, new Vector3(-6, 0.25f, 0));
            case LootPoint.Scp106Outside:
                return new LootPointData(RoomName.Hcz106, new Vector3(-4.102f, 0.573f, 2.733f));
            case LootPoint.Scp106Inside:
                return new LootPointData(RoomName.Hcz106, new Vector3(19.48f, 1.171f, -7.396f));
            case LootPoint.Scp939Desk:
                return new LootPointData(RoomName.Hcz939, new Vector3(0.155f, 1.177f, 2.894f));
            case LootPoint.Scp939Containment:
                return new LootPointData(RoomName.Hcz939, new Vector3(-3.008f, 0.25f, -2.991f));
            case LootPoint.HczCheckpointDesk:
                return new LootPointData(RoomName.HczCheckpointToEntranceZone, new Vector3(-4.34f, 0.808f, -5.593f));
            case LootPoint.HczCheckpointHcz:
                return new LootPointData(RoomName.HczCheckpointToEntranceZone, new Vector3(-3.31f, 0.25f, 5));
            case LootPoint.HczCheckpointEz:
                return new LootPointData(RoomName.HczCheckpointToEntranceZone, new Vector3(4.39f, 0.25f, 2.62f));
            case LootPoint.MicroHIDDesk:
                return new LootPointData(RoomName.HczMicroHID, new Vector3(-4.447f, 5.578f, -2.342f));
            case LootPoint.MicroHIDDeskAlt:
                return new LootPointData(RoomName.HczMicroHID, new Vector3(-6.457f, 5.578f, -2.514f));
            case LootPoint.MicroHIDContainment:
                return new LootPointData(RoomName.HczMicroHID, new Vector3(4.135f, 4.61f, 0.263f));
            case LootPoint.TestRoomDesk:
                return new LootPointData(RoomName.HczTestroom, new Vector3(-0.682f, 0.9f, -4.808f));
            case LootPoint.Lcz173Desk:
                return new LootPointData(RoomName.Lcz173, new Vector3(-2.776f, 12.407f, -5.888f));
            case LootPoint.Lcz173Containment:
                return new LootPointData(RoomName.Lcz173, new Vector3(13.82f, 11.61f, 8.23f));
            case LootPoint.Lcz173Entrance:
                return new LootPointData(RoomName.Lcz173, new Vector3(-5.103f, 0.25f, 1.566f));
            case LootPoint.Lcz173Upstairs:
                return new LootPointData(RoomName.Lcz173, new Vector3(6.05f, 11.61f, 12.5f));
            case LootPoint.Scp330Desk:
                return new LootPointData(RoomName.Lcz173, new Vector3(0.154f, 0.987f, 0.861f));
            case LootPoint.Scp330DeskAlt:
                return new LootPointData(RoomName.Lcz173, new Vector3(2.205f, 0.987f, 0.456f));
            case LootPoint.Scp330Table:
                return new LootPointData(RoomName.Lcz173, new Vector3(0.647f, 0.987f, -2.25f));
            case LootPoint.Gr18Containment:
                return new LootPointData(RoomName.LczGlassroom, new Vector3(4.5f, 0.25f, 2.5f));
            case LootPoint.Gr18Shelve:
                return new LootPointData(RoomName.LczGlassroom, new Vector3(8.371f, 1.056f, -5.914f));
            case LootPoint.Scp914Shelve:
                return new LootPointData(RoomName.Lcz914, new Vector3(0.105f, 1.056f, -7.03f));
            case LootPoint.LczPCsDesk:
                return new LootPointData(RoomName.Lcz914, new Vector3(4.25f, 0.925f, -1.079f));
            case LootPoint.LczPCsDeskAlt:
                return new LootPointData(RoomName.Lcz914, new Vector3(1.08f, 0.925f, 3.3f));
            case LootPoint.WCs:
                return new LootPointData(RoomName.LczToilets, new Vector3(5.34f, 1.062f, -7.095f));
            case LootPoint.WCsAlt:
                return new LootPointData(RoomName.LczToilets, new Vector3(-5.27f, 1.062f, -6.23f));
            default:
                return null;
        }
    }
}