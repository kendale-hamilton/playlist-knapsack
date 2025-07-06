import { createClient } from '@supabase/supabase-js'

const supabaseUrl = process.env.NEXT_PUBLIC_SUPABASE_URL!
const supabaseAnonKey = process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY!

export const supabase = createClient(supabaseUrl, supabaseAnonKey)

// Database types for TypeScript
export interface Database {
  public: {
    Tables: {
      users: {
        Row: {
          id: string
          email: string
          display_name: string
          avatar_url: string | null
          spotify_user_id: string | null
          spotify_access_token: string | null
          spotify_refresh_token: string | null
          created_at: string
          updated_at: string
        }
        Insert: {
          id?: string
          email: string
          display_name: string
          avatar_url?: string | null
          spotify_user_id?: string | null
          spotify_access_token?: string | null
          spotify_refresh_token?: string | null
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          email?: string
          display_name?: string
          avatar_url?: string | null
          spotify_user_id?: string | null
          spotify_access_token?: string | null
          spotify_refresh_token?: string | null
          created_at?: string
          updated_at?: string
        }
      }
    }
  }
} 