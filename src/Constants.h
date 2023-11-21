#ifndef CC_CONSTANTS_H
#define CC_CONSTANTS_H


// If STRING_SIZE is defined elsewhere, comment this line out
#define STRING_SIZE 256 // Define STRING_SIZE


#define GAME_MAX_CMDARGS 5
#define GAME_APP_VER "0.2b"
#define GAME_API_VER 1

#if defined CC_BUILD_WEB
#define GAME_APP_ALT   "NovaCraft 1.1 web mobile"
#define GAME_APP_NAME  "NovaCraft 1.1 web"
#define GAME_APP_TITLE "NovaCraft"
#elif defined CC_BUILD_ANDROID
#define GAME_APP_NAME  "NovaCraft 1.1 android alpha"
#define GAME_APP_TITLE "NovaCraft 0.3b android alpha"
#elif defined CC_BUILD_IOS
#define GAME_APP_NAME  "NovaCraft 1.1 iOS alpha"
#define GAME_APP_TITLE "NovaCraft 1.1 iOS alpha"
#else
#define GAME_APP_NAME  "ClassiCube 1.3.5"
#define GAME_APP_TITLE "NovaCraft 1.1"
#endif

/* Max number of characters strings can have. */
#define STRING_SIZE 64
/* Max number of characters filenames can have. */
#define FILENAME_SIZE 260

/* Chunk axis length in blocks. */
#define CHUNK_SIZE 16
#define HALF_CHUNK_SIZE 8
#define CHUNK_SIZE_2 (CHUNK_SIZE * CHUNK_SIZE)
#define CHUNK_SIZE_3 (CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE)

#define CHUNK_MAX 15
/* Local index in a chunk for a coordinate. */
#define CHUNK_MASK 15
/* Chunk index for a coordinate. */
#define CHUNK_SHIFT 4

/* Chunk axis length (plus neighbours) in blocks. */
#define EXTCHUNK_SIZE 18
#define EXTCHUNK_SIZE_2 (EXTCHUNK_SIZE * EXTCHUNK_SIZE)
#define EXTCHUNK_SIZE_3 (EXTCHUNK_SIZE * EXTCHUNK_SIZE * EXTCHUNK_SIZE)

/* Minor adjustment to max UV coords, to avoid pixel bleeding errors due to rounding. */
#define UV2_Scale (15.99f / 16.0f)

#define GAME_DEF_TICKS (1.0 / 20)
#define GAME_NET_TICKS (1.0 / 60)

#define GUI_MAX_CHATLINES 30

enum FACE_CONSTS {
	FACE_XMIN = 0, /* Face X = 0 */
	FACE_XMAX = 1, /* Face X = 1 */
	FACE_ZMIN = 2, /* Face Z = 0 */
	FACE_ZMAX = 3, /* Face Z = 1 */
	FACE_YMIN = 4, /* Face Y = 0 */
	FACE_YMAX = 5, /* Face Y = 1 */
	FACE_COUNT= 6  /* Number of faces on a cube */
};

enum SKIN_TYPE { SKIN_64x32, SKIN_64x64, SKIN_64x64_SLIM, SKIN_INVALID = 0xF0 };
#define DRAWER2D_MAX_COLORS 256

#define UInt8_MaxValue  ((cc_uint8)255)
#define Int16_MaxValue  ((cc_int16)32767)
#define UInt16_MaxValue ((cc_uint16)65535)
#define Int32_MinValue  ((cc_int32)-2147483647L - (cc_int32)1L)
#define Int32_MaxValue  ((cc_int32)2147483647L)

/* Skins were moved to use Amazon S3, so link directly to avoid a pointless redirect */
#define SKINS_SERVER    "http://cdn.classicube.net/skin"
#define UPDATES_SERVER  "http://cs.classicube.net/client"
#define SERVICES_SERVER "https://www.classicube.net/api"
#define RESOURCE_SERVER "http://static.classicube.net"
/* Webpage where users can register for a new account */
#define REGISTERNEW_URL ""

#define DEFAULT_USERNAME "Singleplayer"
#endif
