/**
 * Interface for the CursorBasedResult returned by `getTables`.
 */
export interface CursorBasedResult {
  firstCursor: string | null
  lastCursor: string | null
  items: Array<Record<string, unknown>>
}
