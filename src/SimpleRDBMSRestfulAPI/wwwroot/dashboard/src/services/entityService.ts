import type { CursorBasedResult } from '@/utils/CursorBasedResult'
import { useServiceEndpoint } from '@/utils/serviceEndpoint'
import msgpack from 'msgpack-lite'

const serviceEndpointHandler = useServiceEndpoint()

/**
 * Fetch tables with cursor-based pagination.
 * @param query The search query for filtering table names (optional).
 * @param rel The cursor direction (0 for previous, 1 for next).
 * @param cursor The base64-encoded cursor string (optional).
 * @param limit The number of items per page.
 * @param offset The number of items to skip (default is 0).
 * @returns A `CursorBasedResult` containing the items and cursors for pagination.
 */
export const getTables = async (
  connectionId: string,
  query: string | null = null,
  rel: number = 1,
  cursor: string | null = null,
  limit: number = 100,
  offset: number = 0,
): Promise<CursorBasedResult> => {
  const apiUrl = serviceEndpointHandler.normalize(`api/entity/${connectionId}/tables`)

  const urlParams = new URLSearchParams({
    rel: rel.toString(),
    limit: limit.toString(),
    offset: offset.toString(),
  })

  if (query) {
    urlParams.append('q', query)
  }
  if (cursor) {
    urlParams.append('cursor', cursor)
  }

  const response = await fetch(`${apiUrl}?${urlParams.toString()}`, {
    method: 'GET',
    headers: {
      Accept: 'application/x-msgpack',
    },
  })

  if (!response.ok) {
    const { errorMessage } = await response.json()
    throw new Error(errorMessage)
  }

  // Decode the msgpack response
  const responseData = await response.arrayBuffer()
  return msgpack.decode(new Uint8Array(responseData))
}

/**
 * Fetch table details from the server using a Base64 encoded string.
 * @param detailEncoded The Base64 encoded detail string.
 * @returns A Promise containing the unpacked response data.
 */
export const getTableDetails = async (connectionId: string,detailEncoded: string): Promise<any> => {
  const apiUrl = serviceEndpointHandler.normalize(`api/entity/${connectionId}/tables/detail`)

  const response = await fetch(`${apiUrl}?detailEncoded=${detailEncoded}`, {
    method: 'GET',
    headers: {
      Accept: 'application/x-msgpack',
    },
  })

  if (!response.ok) {
    const { errorMessage } = await response.json()
    throw new Error(errorMessage)
  }

  // Decode the msgpack response
  const responseData = await response.arrayBuffer()
  return msgpack.decode(new Uint8Array(responseData))
}
